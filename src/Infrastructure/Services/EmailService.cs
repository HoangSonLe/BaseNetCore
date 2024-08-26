using Core.CommonModels;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }
        public async Task SendEmailAsync(string toEmail, string? subject = "MAIL NHẮC NHỞ CÚNG GIỖ", string message = "")
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

            try
            {
                using (var smtp = new SmtpClient())
                {
                    await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                }
            }
            catch (SmtpCommandException ex)
            {
                _logger.LogError($"Failed to send email. Command: {ex.StatusCode} - Error: {ex.Message}");
                // Handle command-specific error
            }
            catch (SmtpProtocolException ex)
            {
                _logger.LogError($"Protocol error occurred while sending email: {ex.Message}");
                // Handle protocol-specific error
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while sending email: {ex.Message}");
                // Handle unexpected errors
            }
        }
    }
}
