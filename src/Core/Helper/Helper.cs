using Core.CommonModels;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;

namespace Core.Helper
{
    public static class Helper
    {
        public static Bitmap GenerateQrCode(string url)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                return qrCode.GetGraphic(20); // 20 is the pixel size
            }
        }
        public static byte[] ResizeImage(byte[] imageData, int width, int height)
        {
            using (var ms = new MemoryStream(imageData))
            {
                using (var originalImage = Image.FromStream(ms))
                {
                    var resizedImage = new Bitmap(width, height);

                    using (var graphics = Graphics.FromImage(resizedImage))
                    {
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                        graphics.DrawImage(originalImage, 0, 0, width, height);
                    }

                    using (var outputMs = new MemoryStream())
                    {
                        resizedImage.Save(outputMs, originalImage.RawFormat);
                        return outputMs.ToArray();
                    }
                }
            }
        }

        public static string GenerateQrCodeAsBase64(string url)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);

                using (var bitmap = qrCode.GetGraphic(20))
                using (var memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    byte[] imageBytes = memoryStream.ToArray();
                    return "data:image/png;base64," + Convert.ToBase64String(imageBytes);
                }
            }
        }
        public static async Task<string> ProcessTelegramResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON string into the TelegramResponse object
                var telegramResponse = JsonSerializer.Deserialize<TelegramResponse>(jsonResponse);

                // Access the telegramChatId and other data
                string telegramChatId = telegramResponse.TelegramChatId;
                string sentMessage = telegramResponse.SentMessage;
                return telegramChatId;
                //Console.WriteLine($"Message sent successfully to Chat ID: {telegramChatId}");
                //Console.WriteLine($"Message Content: {sentMessage}");
            }
            else
            {
                // Handle errors
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<TelegramResponse>(jsonResponse);

                //Console.WriteLine($"Failed to send message: {errorResponse.ErrorMessage}");
                return "";
            }
        }
    }

}
