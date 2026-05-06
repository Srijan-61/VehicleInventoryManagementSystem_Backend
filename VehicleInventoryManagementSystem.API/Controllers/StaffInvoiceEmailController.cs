using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.DTOs.Invoices;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    /// <summary>
    /// Allows staff to send sales invoices to customers through email.
    /// </summary>
    [ApiController]
    [Route("api/staff/invoices")]
    [Authorize(Roles = "Staff")]
    public class StaffInvoiceEmailController : ControllerBase
    {
        private readonly IInvoiceEmailService _invoiceEmailService;
        private readonly ILogger<StaffInvoiceEmailController> _logger;

        public StaffInvoiceEmailController(
            IInvoiceEmailService invoiceEmailService,
            ILogger<StaffInvoiceEmailController> logger)
        {
            _invoiceEmailService = invoiceEmailService;
            _logger = logger;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendInvoiceEmail([FromBody] SendInvoiceEmailRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _invoiceEmailService.SendInvoiceEmailAsync(request.Sales_Invoice_No);

                return Ok(new
                {
                    message = "Invoice email sent successfully to customer."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending invoice email.");

                return StatusCode(500, new
                {
                    message = "Unable to send invoice email. Please try again later."
                });
            }
        }
    }
}