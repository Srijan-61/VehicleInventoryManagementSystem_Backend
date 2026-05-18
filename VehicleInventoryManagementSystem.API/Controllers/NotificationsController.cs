using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class NotificationsController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public NotificationsController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        /// <summary>
        /// Get all low stock alerts (stock < 10 units)
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockAlerts()
        {
            var alerts = await _alertService.GetLowStockAlertsAsync();
            return Ok(alerts);
        }

        /// <summary>
        /// Get all customers with overdue credit (>30 days)
        /// </summary>
        [HttpGet("overdue-credits")]
        public async Task<IActionResult> GetOverdueCredits([FromQuery] int daysOverdue = 30)
        {
            var overdue = await _alertService.GetOverdueCreditsAsync(daysOverdue);
            return Ok(overdue);
        }

        /// <summary>
        /// Send credit reminder to a specific customer
        /// </summary>
        [HttpPost("send-reminder/{customerId}")]
        public async Task<IActionResult> SendCreditReminder(int customerId)
        {
            var result = await _alertService.SendCreditReminderToCustomerAsync(customerId);
            if (result)
                return Ok(new { message = "Reminder sent successfully" });

            return BadRequest(new { message = "Failed to send reminder. Customer may not have credit balance." });
        }

        /// <summary>
        /// Send reminders to all overdue customers
        /// </summary>
        [HttpPost("send-all-reminders")]
        public async Task<IActionResult> SendAllReminders()
        {
            var result = await _alertService.SendAllOverdueRemindersAsync();
            return Ok(new { message = "Reminders processed", sent = result });
        }

        /// <summary>
        /// Manually trigger low stock check
        /// </summary>
        [HttpPost("check-low-stock")]
        public async Task<IActionResult> CheckLowStock()
        {
            await _alertService.CheckAndCreateLowStockAlertsAsync();
            return Ok(new { message = "Low stock check completed" });
        }
    }
}