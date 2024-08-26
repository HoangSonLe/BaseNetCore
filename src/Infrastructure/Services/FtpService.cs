using Core.CoreUtils;
using Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Infrastructure.Services
{
    public class FTPService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private static string hostFTP;
        private static string userNameFTP;
        private static string passwordFTP;
        private static string portFTP;

        public FTPService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;

            userNameFTP = Utils.DecodePassword(_configuration.GetSection("FTP:UserName").Value, EEncodeType.SHA_256);
            passwordFTP = Utils.DecodePassword(_configuration.GetSection("FTP:Password").Value, EEncodeType.SHA_256);
            hostFTP = _configuration.GetSection("FTP:Host").Value;
            portFTP = _configuration.GetSection("FTP:Port").Value;
        }

        public string SaveFTPFile(byte[] fileBytes, string fileName)
        {
            try
            {
                var pathString = string.Format("{0}/{1}/{2}/", "ZaloResize", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.Hour);
                var checkDirectory = hostFTP + ":" + portFTP + "/";
                foreach (var item in pathString.Split("/"))
                {
                    checkDirectory += item + "/";
                    if (!GetDirectoryExits(checkDirectory, userNameFTP, passwordFTP))
                    {
                        CreateDirectory(checkDirectory, userNameFTP, passwordFTP);
                    }
                }

                using var client = new WebClient();
                client.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                client.UploadData(checkDirectory + "/" + fileName, fileBytes);
                return pathString + fileName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string SaveFTPFileImage(byte[] fileBytes, string fileName, string pathString)
        {
            try
            {
                //var pathString = string.Format("{0}/{1}/{2}/", "XLVPGT", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.Hour);
                var checkDirectory = hostFTP + ":" + portFTP + "/";
                foreach (var item in pathString.Split("/"))
                {
                    checkDirectory += item + "/";
                    if (!GetDirectoryExits(checkDirectory, userNameFTP, passwordFTP))
                    {
                        CreateDirectory(checkDirectory, userNameFTP, passwordFTP);
                    }
                }

                using var client = new WebClient();
                client.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                string ftpPath = hostFTP + ":" + portFTP + "//" + (pathString + "/").Replace("/", "//") + fileName;
                client.UploadData(ftpPath, fileBytes);
                return "/" + pathString + "/" + fileName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string UploadedFile(string fileImageBase64, string eventId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileImageBase64))
                    return null;

                string fileName = string.Format("{0}_{1}.jpeg", Guid.NewGuid().ToString(), eventId);
                byte[] fileBytes = Convert.FromBase64String(fileImageBase64);

                var pathString = string.Format("{0}/{1}/{2}/", "Snapshot", DateTime.Now.ToString("yyyy-MM-dd"), "upload");
                var checkDirectory = hostFTP + ":" + portFTP + "/";
                foreach (var item in pathString.Split("/"))
                {
                    checkDirectory += item + "/";
                    if (!GetDirectoryExits(checkDirectory, userNameFTP, passwordFTP))
                    {
                        CreateDirectory(checkDirectory, userNameFTP, passwordFTP);
                    }
                }

                using var client = new WebClient();
                client.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                client.UploadData(checkDirectory + "/" + fileName, fileBytes);
                return pathString + fileName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public byte[] DownloadFile(string fileNamePath)
        {
            try
            {
                byte[] fileData = null;

                string ftpfullpath = string.Format("{0}:{1}/{2}", hostFTP, portFTP, fileNamePath);
                using (WebClient request = new())
                {
                    if (!string.IsNullOrEmpty(userNameFTP) || !string.IsNullOrEmpty(passwordFTP))
                        request.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                    fileData = request.DownloadData(ftpfullpath);
                }

                if (fileData != null && fileData.Length > 0)
                    return fileData;
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public bool GetDirectoryExits(string path, string userNameFTP, string passwordFTP)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                long size = response.ContentLength;
                response.Close();

                return true;
            }
            catch (WebException e)
            {
                _logger.LogInformation("CreateDirectory Status: {0}", e.Message);
                return false;
            }
        }

        public bool CreateDirectory(string path, string userNameFTP, string passwordFTP)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                using var resp = (FtpWebResponse)request.GetResponse();
                if (resp.StatusCode == FtpStatusCode.PathnameCreated)
                {
                    resp.Close();
                    return true;
                }
                resp.Close();
                return false;

            }
            catch (WebException e)
            {
                string status = ((FtpWebResponse)e.Response).StatusDescription;
                _logger.LogError(status);
                return false;
            }
        }

        public string ConvertImageToBase64(byte[] imageBytes)
        {
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }


        public string GetImageAsBase64Url(string url)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using var client = new HttpClient(clientHandler);
            var bytes = client.GetByteArrayAsync(url).Result;
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Hàm này sẽ lưu image vào folder không giống những hàm khác của FTP
        /// </summary>
        /// <param name="base64Image"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public string SaveImageInternalDB(string base64Image, string Id, string folderName = "Tmp")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(base64Image))
                    return null;

                string imageDirectory = _configuration.GetSection("FolderPathImage").Value;
                if (imageDirectory == null)
                {
                    _logger.LogError("SaveImage: FolderPathImage not config");
                    return null;
                }

                byte[] imageBytes = Convert.FromBase64String(base64Image);
                string pathString = string.Format("{0}/{1}/", folderName, DateTime.Now.ToString("yyyy-MM-dd"));
                string directoryPath = Path.Combine(imageDirectory, pathString);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                // Kiểm tra nếu thư mục chưa tồn tại thì tạo mới
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string imageName = string.Format("{0}_{1}.jpeg", Guid.NewGuid().ToString(), Id);
                string imagePath = Path.Combine(directoryPath, imageName);

                File.WriteAllBytes(imagePath, imageBytes);

                return "/" + pathString + imageName;
            }
            catch (Exception ex)
            {
                _logger.LogError("SaveImage: " + ex.Message);
                return null;
            }
        }
    }
}
