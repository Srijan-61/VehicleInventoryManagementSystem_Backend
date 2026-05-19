using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Infrastructure.Settings;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    /// <summary>
    /// Sends emails using SMTP configuration from appsettings.json.
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
            ValidateEmailRequest(toEmail, subject, htmlBody);
            ValidateSmtpSettings();

            try
            {
                using var message = BuildMailMessage(toEmail, subject, htmlBody);
                using var smtpClient = BuildSmtpClient();

                await smtpClient.SendMailAsync(message);

                _logger.LogInformation("Email sent successfully to {Email}.", toEmail);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error while sending email to {Email}.", toEmail);
                throw new InvalidOperationException("Email could not be sent due to SMTP server error.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending email to {Email}.", toEmail);
                throw;
            }
        }

        private static void ValidateEmailRequest(
            string toEmail,
            string subject,
            string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email address is required.");

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Email subject is required.");

            if (string.IsNullOrWhiteSpace(htmlBody))
                throw new ArgumentException("Email body is required.");
        }

        private void ValidateSmtpSettings()
        {
            if (string.IsNullOrWhiteSpace(_settings.Host))
                throw new InvalidOperationException("SMTP host is not configured.");

            if (_settings.Port <= 0)
                throw new InvalidOperationException("SMTP port is not configured properly.");

            if (string.IsNullOrWhiteSpace(_settings.SenderEmail))
                throw new InvalidOperationException("SMTP sender email is not configured.");

            if (string.IsNullOrWhiteSpace(_settings.Password))
                throw new InvalidOperationException("SMTP password is not configured.");

            if (string.IsNullOrWhiteSpace(_settings.SenderName))
                _settings.SenderName = "Vehicle Inventory Management System";
        }

        private MailMessage BuildMailMessage(
            string toEmail,
            string subject,
            string htmlBody)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            return message;
        }

        private SmtpClient BuildSmtpClient()
        {
            return new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(
                    _settings.SenderEmail,
                    _settings.Password
                ),
                EnableSsl = _settings.EnableSsl
            };
        }
    }
}