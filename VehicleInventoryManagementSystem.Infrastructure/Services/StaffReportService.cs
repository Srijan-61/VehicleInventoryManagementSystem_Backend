using VehicleInventoryManagementSystem.Application.DTOs.Reports;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    /// <summary>
    /// Handles business logic for staff reports.
    /// </summary>
    public class StaffReportService : IStaffReportService
    {
        private const int DefaultLimit = 10;
        private const int MaximumLimit = 100;
        private const int DefaultMinimumPurchases = 2;

        private readonly IStaffReportRepository _staffReportRepository;

        public StaffReportService(IStaffReportRepository staffReportRepository)
        {
            _staffReportRepository = staffReportRepository;
        }

        public async Task<List<RegularCustomerReportDto>> GetRegularCustomersAsync(int minimumPurchases, int limit)
        {
            minimumPurchases = minimumPurchases <= 0 ? DefaultMinimumPurchases : minimumPurchases;
            limit = NormalizeLimit(limit);

            return await _staffReportRepository.GetRegularCustomersAsync(minimumPurchases, limit);
        }

        public async Task<List<HighSpenderReportDto>> GetHighSpendersAsync(int limit)
        {
            limit = NormalizeLimit(limit);

            return await _staffReportRepository.GetHighSpendersAsync(limit);
        }

        public async Task<List<PendingCreditReportDto>> GetPendingCreditsAsync()
        {
            return await _staffReportRepository.GetPendingCreditsAsync();
        }

        private static int NormalizeLimit(int limit)
        {
            if (limit <= 0)
                return DefaultLimit;

            return limit > MaximumLimit ? MaximumLimit : limit;
        }
    }
}