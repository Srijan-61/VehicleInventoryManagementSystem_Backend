using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VehicleInventoryManagementSystem.Application.DTOs.Notifications;
using VehicleInventoryManagementSystem.Application.Interfaces.Notifications;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Services.Notifications
{
    /// <summary>
    /// Implements F15 alerting. Currently exposes the low-stock report;
    /// email reminders for overdue credits will be added once
    /// CreditReminderLog migration is in.
    /// </summary>
    public class NotificationService : INotificationService
    {
        /// <summary>
        /// Default low-stock threshold per the coursework brief.
        /// </summary>
        private const int DefaultLowStockThreshold = 10;

        private readonly AppDbContext _dbContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            AppDbContext dbContext,
            ILogger<NotificationService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<LowStockReportResponse> GetLowStockReportAsync(
            int? threshold = null,
            CancellationToken cancellationToken = default)
        {
            var effectiveThreshold = threshold ?? DefaultLowStockThreshold;

            // Defensive: a negative threshold would return nothing useful.
            if (effectiveThreshold < 0)
            {
                effectiveThreshold = 0;
            }

            var lowStockItems = await _dbContext.VehicleParts
                .AsNoTracking()
                .Where(p => p.Stock_Quantity < effectiveThreshold)
                .OrderBy(p => p.Stock_Quantity)
                .ThenBy(p => p.Part_Name)
                .Select(p => new LowStockPartResponse
                {
                    PartId = p.Part_ID,
                    PartName = p.Part_Name,
                    PartCategory = p.Part_Category,
                    Brand = p.Brand,
                    StockQuantity = p.Stock_Quantity,
                    MinimumStockLevel = p.Minimum_Stock_Level
                })
                .ToListAsync(cancellationToken);

            var report = new LowStockReportResponse
            {
                Threshold = effectiveThreshold,
                LowStockCount = lowStockItems.Count,
                OutOfStockCount = lowStockItems.Count(i => i.StockQuantity <= 0),
                GeneratedAt = DateTime.UtcNow,
                Items = lowStockItems
            };

            _logger.LogInformation(
                "Low-stock report generated: {LowCount} part(s) below threshold {Threshold} " +
                "({OutOfStockCount} out of stock).",
                report.LowStockCount, effectiveThreshold, report.OutOfStockCount);

            return report;
        }
    }
}