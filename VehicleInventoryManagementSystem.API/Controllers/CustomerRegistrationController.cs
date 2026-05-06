using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    // This controller handles customer registration along with their vehicle info (Feature 6)
    // Only Staff members can register new customers, and it uses a dedicated service for this
    [Route("api/staff/register-customer")]
    [ApiController]
    [Authorize(Roles = "Staff")]
    public class CustomerRegistrationController : ControllerBase
    {
        private readonly ICustomerRegistrationService _customerRegistrationService;

        public CustomerRegistrationController(ICustomerRegistrationService customerRegistrationService)
        {
            _customerRegistrationService = customerRegistrationService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterCustomerWithVehicle([FromBody] RegisterCustomerWithVehicleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (succeeded, errors) = await _customerRegistrationService.RegisterCustomerWithVehicleAsync(dto);

            if (succeeded)
                return Ok(new { Message = "Customer and vehicle registered successfully." });

            return BadRequest(new { Message = "Registration failed.", Errors = errors });
        }
    }
}
