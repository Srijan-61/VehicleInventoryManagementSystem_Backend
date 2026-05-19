namespace VehicleInventoryManagementSystem.Infrastructure.Settings
{
    /// <summary>
    /// Represents SMTP configuration values from appsettings.json.
    /// This is a settings class, not a service.
    /// </summary>
    public class SmtpEmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }
}