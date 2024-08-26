using Application.Base;
using Application.Services.WebInterfaces;
using Core.CommonModels.BaseModels;
using Core.CoreUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseWebsite.Controllers
{
    [Authorize]
    public class FileController : BaseController<FileController>
    {
        private readonly IUserService _userService;

        private readonly IConfiguration _configuration;
        private readonly ILogger<FileController> _logger;
        private string _rootDirectory;

        public FileController(
            IUserService userService,
            ILogger<FileController> logger,
            IConfiguration configuration
            ) : base(logger, userService)
        {
            _userService = userService;
            _configuration = configuration;
            _logger = logger;
            _rootDirectory = _configuration.GetSection("FolderPathImage").Value;
        }
        [HttpPost]
        public async Task<Acknowledgement<UploadFileResponse>> UploadFile(IFormFile file, string prefixFolderPath = "URN")
        {
            IFormFile[] files = new IFormFile[]
            {
                file
            };
            var ack = await UploadFiles(files, prefixFolderPath);
            if (ack.IsSuccess)
            {
                return new Acknowledgement<UploadFileResponse>()
                {
                    IsSuccess = true,
                    Data = ack.Data.First()
                };
            }
            return new Acknowledgement<UploadFileResponse>()
            {
                IsSuccess = false,
                ErrorMessageList = ack.ErrorMessageList
            };
        }
        public class UploadFileResponse
        {
            public string Path { get; set; }
            public string FileName { get; set; }
        }
        [HttpPost]
        public async Task<Acknowledgement<List<UploadFileResponse>>> UploadFiles(IFormFile[] files, string prefixFolderPath = "URN")
        {
            var ack = new Acknowledgement<List<UploadFileResponse>>()
            {
                Data = new List<UploadFileResponse>()
            };
            try
            {
                string now = DateTime.Now.ToString("dd-MM-yyyy");
                string path = "";

                if (files != null && files.Length > 0)
                {
                    foreach (var item in files)
                    {
                        var fileName = DateTime.Now.ToString("MM_dd_yyyy_HH_mm") + "_" + item.FileName.Substring(item.FileName.LastIndexOf("\\") + 1);
                        string pathfolder = $"{prefixFolderPath}";
                        path = $"{_rootDirectory}/{pathfolder}";
                        var saveAck = await FtpFunction.SaveFileToFolder(item, _rootDirectory, pathfolder, fileName);
                        if (saveAck.IsSuccess == false)
                        {
                            _logger.LogError("SaveFile ", string.Join(",", saveAck.ErrorMessageList));
                            ack.ErrorMessageList = saveAck.ErrorMessageList;
                            return ack;
                        }
                        var filePath = saveAck.Data;
                        ack.Data.Add(new UploadFileResponse()
                        {
                            Path = filePath,
                            FileName = fileName,
                        });

                    }
                }
                else
                {
                    ack.AddMessage("Không tìm thấy tệp, vui lòng kiểm tra lại.");
                    return ack;
                }
                ack.IsSuccess = true;
                return ack;

            }
            catch (Exception ex)
            {
                ack.ExtractMessage(ex);
                return ack;
            }
        }
        [HttpGet("/Files/{*folderFilePath}")]
        public IActionResult DownloadFile(string folderFilePath)
        {
            var filePath = Path.Combine(_rootDirectory, folderFilePath);
            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                //var fileName = Path.GetFileName(filePath);
                var mimeType = GetMimeType(filePath);
                long fileSizeInBytes = fileBytes.Length;

                // Convert bytes to megabytes (1 MB = 1024 * 1024 bytes)
                double fileSizeInMB = (double)fileSizeInBytes / (1024 * 1024);
                var quality = 100;
                // You can now use fileSizeInMB for validation or any other purpose
                if (fileSizeInMB > 2) // Example: Check if file size exceeds 5 MB
                {
                    quality = 50;
                }
                var result = FtpFunction.CompressImageWithResize(fileBytes, 800, 640, quality);
                if (result == null)
                {
                    return BadRequest("Invalid image data.");
                }
                return File(result.ImageBytes, result.ContentType);
            }
            else
            {
                return NotFound(); // Return 404 if the file is not found
            }
        }
        [HttpGet("/FilesWithLowQuality/{*folderFilePath}")]
        public IActionResult DownloadFileWithLowQuality(string folderFilePath)
        {
            var filePath = Path.Combine(_rootDirectory, folderFilePath);
            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                long fileSizeInBytes = fileBytes.Length;

                // Convert bytes to megabytes (1 MB = 1024 * 1024 bytes)
                double fileSizeInMB = (double)fileSizeInBytes / (1024 * 1024);
                var quality = 100;
                // You can now use fileSizeInMB for validation or any other purpose
                if (fileSizeInMB > 2) // Example: Check if file size exceeds 5 MB
                {
                    quality = 50;
                }
                var result = FtpFunction.CompressImageWithResize(fileBytes, 200, 160, quality);
                if (result == null)
                {
                    return BadRequest("Invalid image data.");
                }
                return File(result.ImageBytes, result.ContentType);
            }
            else
            {
                return NotFound(); // Return 404 if the file is not found
            }
        }



        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                _ => "application/octet-stream", // Default for unknown file types
            };
        }

    }
}
