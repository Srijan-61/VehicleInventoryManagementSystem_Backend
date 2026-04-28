using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.DTOs.Notifications;
using VehicleInventoryManagementSystem.Application.Interfaces.Notifications;

namespace VehicleInventoryManagementSystem.API.Controllers.Notifications
{
    [ApiController]
    [Route("api/notifications")]
    // TODO: [Authorize(Roles = "Admin")] once JWT is wired up.
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        /// <summary>
        /// F15 — Low-stock alert feed for the admin dashboard.
        /// </summary>
        /// <param name="threshold">
        /// Optional override for the stock threshold. Defaults to 10.
        /// </param>
        [HttpGet("low-stock")]
        [ProducesResponseType(typeof(LowStockReportResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLowStock(
            [FromQuery] int? threshold,
            CancellationToken cancellationToken)
        {
            var report = await _service.GetLowStockReportAsync(threshold, cancellationToken);
            return Ok(report);
        }
    }
}