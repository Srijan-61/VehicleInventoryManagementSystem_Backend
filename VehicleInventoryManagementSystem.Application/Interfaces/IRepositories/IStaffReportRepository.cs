using VehicleInventoryManagementSystem.Application.DTOs.Reports;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface IStaffReportRepository
    {
        Task<List<RegularCustomerReportDto>> GetRegularCustomersAsync(
            int minimumPurchases,
            int limit,
            DateTime? startDate,
            DateTime? endDate
        );

        Task<List<HighSpenderReportDto>> GetHighSpendersAsync(
            int limit,
            decimal? minimumSpent,
            DateTime? startDate,
            DateTime? endDate
        );

        Task<List<PendingCreditReportDto>> GetPendingCreditsAsync(
            bool overdueOnly
        );
    }
}