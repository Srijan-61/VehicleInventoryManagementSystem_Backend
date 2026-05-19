using VehicleInventoryManagementSystem.Application.DTOs.Reports;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    /// <summary>
    /// Handles business rules and validation for staff reports.
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

        public async Task<List<RegularCustomerReportDto>> GetRegularCustomersAsync(
            int minimumPurchases,
            int limit,
            DateTime? startDate,
            DateTime? endDate)
        {
            minimumPurchases = minimumPurchases <= 0
                ? DefaultMinimumPurchases
                : minimumPurchases;

            limit = NormalizeLimit(limit);

            ValidateDateRange(startDate, endDate);

            return await _staffReportRepository.GetRegularCustomersAsync(
                minimumPurchases,
                limit,
                startDate,
                endDate
            );
        }

        public async Task<List<HighSpenderReportDto>> GetHighSpendersAsync(
            int limit,
            decimal? minimumSpent,
            DateTime? startDate,
            DateTime? endDate)
        {
            limit = NormalizeLimit(limit);

            if (minimumSpent.HasValue && minimumSpent.Value < 0)
                minimumSpent = 0;

            ValidateDateRange(startDate, endDate);

            return await _staffReportRepository.GetHighSpendersAsync(
                limit,
                minimumSpent,
                startDate,
                endDate
            );
        }

        public async Task<List<PendingCreditReportDto>> GetPendingCreditsAsync(
            bool overdueOnly)
        {
            return await _staffReportRepository.GetPendingCreditsAsync(overdueOnly);
        }

        private static int NormalizeLimit(int limit)
        {
            if (limit <= 0)
                return DefaultLimit;

            return limit > MaximumLimit ? MaximumLimit : limit;
        }

        private static void ValidateDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue &&
                endDate.HasValue &&
                startDate.Value.Date > endDate.Value.Date)
            {
                throw new ArgumentException("Start date cannot be after end date.");
            }
        }
    }
}