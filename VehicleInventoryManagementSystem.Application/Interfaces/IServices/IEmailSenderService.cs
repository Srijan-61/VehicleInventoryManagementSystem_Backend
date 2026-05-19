namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    /// <summary>
    /// Defines contract for sending emails.
    /// </summary>
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}