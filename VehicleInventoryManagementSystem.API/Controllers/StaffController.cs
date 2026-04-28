using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // FEATURE 7 & 16: Create Sales Invoice with Loyalty Discount
        [HttpPost("create-sales-invoice")]
        public async Task<IActionResult> CreateSalesInvoice([FromBody] CreateSalesInvoiceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (succeeded, message, errors) = await _staffService.CreateSalesInvoiceAsync(dto);

            if (succeeded)
                return Ok(new { Message = message });

            return BadRequest(new { Message = "Transaction failed.", Errors = errors });
        }
    }
}
