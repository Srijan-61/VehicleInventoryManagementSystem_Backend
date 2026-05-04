using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    /// <summary>
    /// Controller for generating staff-related customer reports.
    /// Only accessible by Staff role.
    /// </summary>
    [ApiController]
    [Route("api/staff/reports")]
    [Authorize(Roles = "Staff")]
    public class StaffReportsController : ControllerBase
    {
        private readonly IStaffReportService _staffReportService;
        private readonly ILogger<StaffReportsController> _logger;

        public StaffReportsController(
            IStaffReportService staffReportService,
            ILogger<StaffReportsController> logger)
        {
            _staffReportService = staffReportService;
            _logger = logger;
        }

        /// <summary>
        /// Returns customers who purchase frequently.
        /// </summary>
        [HttpGet("regular-customers")]
        public async Task<IActionResult> GetRegularCustomers(
            [FromQuery] int minimumPurchases = 2,
            [FromQuery] int limit = 10)
        {
            try
            {
                var report = await _staffReportService.GetRegularCustomersAsync(minimumPurchases, limit);

                return Ok(new
                {
                    message = "Regular customers report generated successfully.",
                    count = report.Count,
                    data = report
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating regular customers report.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        /// <summary>
        /// Returns customers who spent the most.
        /// </summary>
        [HttpGet("high-spenders")]
        public async Task<IActionResult> GetHighSpenders([FromQuery] int limit = 10)
        {
            try
            {
                var report = await _staffReportService.GetHighSpendersAsync(limit);

                return Ok(new
                {
                    message = "High spenders report generated successfully.",
                    count = report.Count,
                    data = report
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating high spenders report.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        /// <summary>
        /// Returns customers with unpaid invoices.
        /// </summary>
        [HttpGet("pending-credits")]
        public async Task<IActionResult> GetPendingCredits()
        {
            try
            {
                var report = await _staffReportService.GetPendingCreditsAsync();

                return Ok(new
                {
                    message = "Pending credits report generated successfully.",
                    count = report.Count,
                    data = report
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating pending credits report.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }
    }
}