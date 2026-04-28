using VehicleInventoryManagementSystem.Application.DTOs.Notifications;

namespace VehicleInventoryManagementSystem.Application.Interfaces.Notifications
{
    /// <summary>
    /// Surfaces system-generated alerts to admins (Feature F15).
    /// Email reminders for overdue credits will live here too once
    /// the supporting schema is in place.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Returns all parts whose stock has fallen below the threshold.
        /// </summary>
        /// <param name="threshold">
        /// Stock-level cutoff. Defaults to the brief's value of 10
        /// when not supplied.
        /// </param>
        Task<LowStockReportResponse> GetLowStockReportAsync(
            int? threshold = null,
            CancellationToken cancellationToken = default);
    }
}