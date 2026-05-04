using VehicleInventoryManagementSystem.Application.DTOs.Reports;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface IStaffReportRepository
    {
        Task<List<RegularCustomerReportDto>> GetRegularCustomersAsync(int minimumPurchases, int limit);
        Task<List<HighSpenderReportDto>> GetHighSpendersAsync(int limit);
        Task<List<PendingCreditReportDto>> GetPendingCreditsAsync();
    }
}