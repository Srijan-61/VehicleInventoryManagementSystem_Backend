using VehicleInventoryManagementSystem.Application.DTOs.Reports;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface IStaffReportService
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