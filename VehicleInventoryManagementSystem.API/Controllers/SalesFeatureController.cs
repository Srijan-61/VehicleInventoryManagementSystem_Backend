using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    // This controller handles the sales and POS functionality (Features 7 and 16)
    // Staff can create invoices and look up customers from here
    [Route("api/sales")]
    [ApiController]
    [Authorize(Roles = "Staff")]
    public class SalesFeatureController : ControllerBase
    {
        private readonly ISalesFeatureService _salesFeatureService;

        public SalesFeatureController(ISalesFeatureService salesFeatureService)
        {
            _salesFeatureService = salesFeatureService;
        }

        // Returns the list of customers so the frontend can show them in a dropdown
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomersDropdown()
        {
            var customers = await _salesFeatureService.GetCustomersForDropdownAsync();
            return Ok(customers);
        }

        // This is the main endpoint for creating a new sales invoice
        [HttpPost("create-invoice")]
        public async Task<IActionResult> CreateSalesInvoice([FromBody] CreateSalesInvoiceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Get the logged-in user's ID from their JWT token so we know which staff member is making the sale
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Look up the Staff_ID from the database using their user ID
            dto.Staff_ID = await _salesFeatureService.GetCurrentStaffIdAsync(userId);
            if (dto.Staff_ID == 0) return BadRequest(new { Message = "Staff profile not found." });

            // Now actually process the sale and get back the invoice details
            var (succeeded, data, errors) = await _salesFeatureService.CreateSalesInvoiceAsync(dto);

            if (succeeded)
                return Ok(data);

            return BadRequest(new { Message = "Transaction failed.", Errors = errors });
        }
    }
}
