using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    /// <summary>
    /// Sends emails using SMTP settings from appsettings.json.
    /// </summary>
    public class SmtpEmailSenderService : IEmailSenderService
    {
        private readonly SmtpEmailSettings _settings;
        private readonly ILogger<SmtpEmailSenderService> _logger;

        public SmtpEmailSenderService(
            IOptions<SmtpEmailSettings> settings,
            ILogger<SmtpEmailSenderService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Customer email address is required.");

            if (string.IsNullOrWhiteSpace(_settings.SenderEmail) ||
                string.IsNullOrWhiteSpace(_settings.Password))
                throw new InvalidOperationException("SMTP email settings are not configured properly.");

            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                message.To.Add(toEmail);

                using var smtpClient = new SmtpClient(_settings.Host, _settings.Port)
                {
                    Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password),
                    EnableSsl = _settings.EnableSsl
                };

                await smtpClient.SendMailAsync(message);

                _logger.LogInformation("Email sent successfully to {Email}.", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}.", toEmail);
                throw;
            }
        }
    }
}