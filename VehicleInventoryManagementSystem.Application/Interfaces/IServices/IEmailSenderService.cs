using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
