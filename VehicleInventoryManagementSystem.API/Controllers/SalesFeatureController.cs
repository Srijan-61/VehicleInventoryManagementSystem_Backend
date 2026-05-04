using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    // Features 7 & 16: Sales & POS - Vertical Slice Controller
    // 1:1:1 Rule => Injects ONLY ISalesFeatureService
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

        // GET api/sales/customers - Fetch customers for dropdown
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomersDropdown()
        {
            var customers = await _salesFeatureService.GetCustomersForDropdownAsync();
            return Ok(customers);
        }

        // POST api/sales/create-invoice - Create a sales invoice (Feature 7 & 16)
        [HttpPost("create-invoice")]
        public async Task<IActionResult> CreateSalesInvoice([FromBody] CreateSalesInvoiceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Securely extract the logged-in User's ID from the JWT Token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Use the internal helper method to find their Staff_ID
            dto.Staff_ID = await _salesFeatureService.GetCurrentStaffIdAsync(userId);
            if (dto.Staff_ID == 0) return BadRequest(new { Message = "Staff profile not found." });

            // Process the sale
            var (succeeded, data, errors) = await _salesFeatureService.CreateSalesInvoiceAsync(dto);

            if (succeeded)
                return Ok(data);

            return BadRequest(new { Message = "Transaction failed.", Errors = errors });
        }
    }
}
