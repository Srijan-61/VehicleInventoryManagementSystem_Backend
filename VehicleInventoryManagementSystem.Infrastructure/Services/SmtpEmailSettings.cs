using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
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