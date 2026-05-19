using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.DTOs.Invoices;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    /// <summary>
    /// Allows staff to manage and send invoice emails.
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

        /// <summary>
        /// Returns all invoices of selected customer.
        /// Used for invoice dropdown.
        /// </summary>
        [HttpGet("customer/{customerId}/invoices")]
        public async Task<IActionResult> GetInvoicesByCustomer(int customerId)
        {
            try
            {
                var invoices = await _invoiceEmailService
                    .GetInvoicesByCustomerAsync(customerId);

                return Ok(invoices);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while loading customer invoices."
                );

                return StatusCode(500, new
                {
                    message = "Unable to load invoices."
                });
            }
        }

        /// <summary>
        /// Returns invoice details for preview.
        /// </summary>
        [HttpGet("customer/{customerId}/invoice/{salesInvoiceNo}")]
        public async Task<IActionResult> GetInvoiceDetails(
            int customerId,
            int salesInvoiceNo)
        {
            try
            {
                var invoice = await _invoiceEmailService
                    .GetInvoiceEmailDetailsAsync(
                        customerId,
                        salesInvoiceNo
                    );

                if (invoice == null)
                {
                    return NotFound(new
                    {
                        message = "Invoice not found for selected customer."
                    });
                }

                return Ok(invoice);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while loading invoice details."
                );

                return StatusCode(500, new
                {
                    message = "Unable to load invoice details."
                });
            }
        }

        /// <summary>
        /// Sends invoice email to customer.
        /// </summary>
        [HttpPost("send-email")]
        public async Task<IActionResult> SendInvoiceEmail(
            [FromBody] SendInvoiceEmailRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _invoiceEmailService.SendInvoiceEmailAsync(request);

                return Ok(new
                {
                    message = "Invoice email sent successfully to customer."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while sending invoice email."
                );

                return StatusCode(500, new
                {
                    message = "Unable to send invoice email. Please try again later."
                });
            }
        }
    }
}