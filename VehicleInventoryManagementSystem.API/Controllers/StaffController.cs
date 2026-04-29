using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Staff")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpPost("register-customer")]
        public async Task<IActionResult> RegisterCustomerWithVehicle([FromBody] RegisterCustomerWithVehicleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (succeeded, errors) = await _staffService.RegisterCustomerWithVehicleAsync(dto);

            if (succeeded)
                return Ok(new { Message = "Customer and vehicle registered successfully." });

            return BadRequest(new { Message = "Registration failed.", Errors = errors });
        }

        // get customer details for dropdown in sales invoice creation form
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomersDropdown()
        {
            var customers = await _staffService.GetCustomersForDropdownAsync();
            return Ok(customers);
        }

        // Feature 7 & 16 
        [HttpPost("create-sales-invoice")]
        public async Task<IActionResult> CreateSalesInvoice([FromBody] CreateSalesInvoiceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Securely extract the logged-in User's ID from the JWT Token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Use the internal helper method to find their Staff_ID
            dto.Staff_ID = await _staffService.GetCurrentStaffIdAsync(userId);
            if (dto.Staff_ID == 0) return BadRequest(new { Message = "Staff profile not found." });

            // Process the sale
            var (succeeded, data, errors) = await _staffService.CreateSalesInvoiceAsync(dto);

            if (succeeded)
                return Ok(data);

            return BadRequest(new { Message = "Transaction failed.", Errors = errors });
        }
    }
}
