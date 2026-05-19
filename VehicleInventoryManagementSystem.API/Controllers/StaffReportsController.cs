using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    /// <summary>
    /// Controller for generating customer reports for staff.
    /// Only Staff role can access these APIs.
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

        // Returns customers who purchase frequently.
        [HttpGet("regular-customers")]
        public async Task<IActionResult> GetRegularCustomers(
            [FromQuery] int minimumPurchases = 2,
            [FromQuery] int limit = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var report = await _staffReportService.GetRegularCustomersAsync(
                    minimumPurchases,
                    limit,
                    startDate,
                    endDate
                );

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

        // Returns customers who spent the most.
        [HttpGet("high-spenders")]
        public async Task<IActionResult> GetHighSpenders(
            [FromQuery] int limit = 10,
            [FromQuery] decimal? minimumSpent = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var report = await _staffReportService.GetHighSpendersAsync(
                    limit,
                    minimumSpent,
                    startDate,
                    endDate
                );

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

        // Returns customers with unpaid invoices or overdue credits.
        [HttpGet("pending-credits")]
        public async Task<IActionResult> GetPendingCredits(
            [FromQuery] bool overdueOnly = false)
        {
            try
            {
                var report = await _staffReportService.GetPendingCreditsAsync(overdueOnly);

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