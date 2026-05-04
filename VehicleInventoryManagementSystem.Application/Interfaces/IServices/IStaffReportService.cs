using VehicleInventoryManagementSystem.Application.DTOs.Reports;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface IStaffReportService
    {
        Task<List<RegularCustomerReportDto>> GetRegularCustomersAsync(int minimumPurchases, int limit);
        Task<List<HighSpenderReportDto>> GetHighSpendersAsync(int limit);
        Task<List<PendingCreditReportDto>> GetPendingCreditsAsync();
    }
}