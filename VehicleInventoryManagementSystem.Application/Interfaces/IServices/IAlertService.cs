using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces
{
    public interface IAlertService
    {
        Task<List<LowStockAlertDto>> GetLowStockAlertsAsync();
        Task<List<OverdueCreditDto>> GetOverdueCreditsAsync(int daysOverdue = 30);
        Task CheckAndCreateLowStockAlertsAsync();
        Task CheckAndSendCreditRemindersAsync();
        Task<bool> SendCreditReminderToCustomerAsync(int customerId);
        Task<bool> SendAllOverdueRemindersAsync();
    }
}