using System.Drawing.Imaging;
using System.Net;
using System.Drawing;
using Core.CommonModels.BaseModels;
using Microsoft.AspNetCore.Http;

namespace Core.CoreUtils
{
    public static class FtpFunction
    {

        public static bool GetDirectoryExits(string path, string userNameFTP, string passwordFTP)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool CreateDirectory(string path, string userNameFTP, string passwordFTP)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                var resp = (FtpWebResponse)request.GetResponse();
                if (resp.StatusCode == FtpStatusCode.PathnameCreated)
                {
                    resp.Close();
                    return true;
                }
                resp.Close();
                return false;

            }
            catch (Exception e)
            {
                return false;
            }

        }
        public class CompressedImageResult
        {
            public byte[] ImageBytes { get; set; }
            public string ContentType { get; set; }
        }
        private static Image ResizeImage(Image imgToResize, Size size)
        {
            return new Bitmap(imgToResize, size);
        }

        public static CompressedImageResult CompressImageWithResize(byte[] fileBytes, int maxWidth, int maxHeight, int jpegQuality = 1)
        {
            using (var inputStream = new MemoryStream(fileBytes))
            using (var originalImage = Image.FromStream(inputStream))
            {
                // Resize the image if it's larger than the max dimensions
                var ratioX = (double)maxWidth / originalImage.Width;
                var ratioY = (double)maxHeight / originalImage.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(originalImage.Width * ratio);
                var newHeight = (int)(originalImage.Height * ratio);

                using (var resizedImage = ResizeImage(originalImage, new Size(newWidth, newHeight)))
                using (var memoryStream = new MemoryStream())
                {

                    var jpegEncoder = GetEncoder(ImageFormat.Jpeg);

                    var encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, jpegQuality);

                    resizedImage.Save(memoryStream, jpegEncoder, encoderParams);
                    // Return the compressed image as a byte array
                    return new CompressedImageResult
                    {
                        ImageBytes = memoryStream.ToArray(),
                        ContentType = "image/jpeg"
                    };
                }
            }
        }

        public static CompressedImageResult CompressImage(byte[] fileBytes)
        {
            if (fileBytes == null || fileBytes.Length == 0)
            {
                return null;
            }

            using (var inputStream = new MemoryStream(fileBytes))
            using (var originalImage = Image.FromStream(inputStream))
            {
                // Set the JPEG quality (e.g., 50% quality)
                var jpegQuality = 80; // Adjust the quality as needed (0-100)

                using (var memoryStream = new MemoryStream())
                {
                    // Get the JPEG codec
                    var jpegEncoder = GetEncoder(ImageFormat.Jpeg);

                    // Set the quality parameter for the encoder
                    var encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, jpegQuality);

                    // Save the compressed image to the memory stream
                    originalImage.Save(memoryStream, jpegEncoder, encoderParams);

                    // Return the compressed image as a byte array
                    return new CompressedImageResult
                    {
                        ImageBytes = memoryStream.ToArray(),
                        ContentType = "image/jpeg"
                    };
                }
            }
        }
        // Helper method to get the image encoder for a specific format
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static async Task<Acknowledgement<string>> SaveFileToFolder(IFormFile imageFile, string rootDirectory, string folderPath, string fileName)
        {
            var ack = new Acknowledgement<string>();

            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    ack.AddMessage("File data is empty");
                    return ack;
                }

                if (folderPath == null)
                {
                    ack.AddMessage("SaveFileToFolder: folderPath is null");
                    return ack;
                }

                // Ensure the directory exists
                if (!Directory.Exists($"{rootDirectory}/{folderPath}"))
                {
                    Directory.CreateDirectory($"{rootDirectory}/{folderPath}");
                }

                // Save the image to the server
                var folderFilePath = $"{folderPath}/{fileName}";
                var path = $"{rootDirectory}/{folderFilePath}";

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Optionally, save the image file path to the model associated with the given ID
                // You can return the image URL or other information

                ack.IsSuccess = true;
                ack.Data = folderFilePath;
                return ack;
            }
            catch (Exception ex)
            {
                ack.ExtractMessage(ex);
                return ack;
            }

        }

        public static Acknowledgement<string> DeleteFileInFolder(string folderFilePath)
        {
            var ack = new Acknowledgement<string>();
            try
            {
                if (File.Exists(folderFilePath))
                {
                    File.Delete(folderFilePath);
                    ack.IsSuccess = true;
                    return ack;
                }
                ack.AddMessages("File not found.");
                return ack;
            }
            catch (Exception ex)
            {
                ack.ExtractMessage(ex);
                return ack;
            }

        }
    }
}
