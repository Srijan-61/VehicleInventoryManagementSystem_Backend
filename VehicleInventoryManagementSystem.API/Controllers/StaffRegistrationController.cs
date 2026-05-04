using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.DTOs.Auth;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    // This controller handles staff registration (Feature 2)
    // Only Admins can access this, it calls the registration service to create a new staff account
    
    [Route("api/staff-registration")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StaffRegistrationController : ControllerBase
    {
        private readonly IStaffRegistrationService _staffRegistrationService;

        public StaffRegistrationController(IStaffRegistrationService staffRegistrationService)
        {
            _staffRegistrationService = staffRegistrationService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStaff([FromBody] RegisterStaffDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (succeeded, errors) = await _staffRegistrationService.RegisterStaffAsync(dto);

            if (succeeded)
                return Ok(new { Message = "Staff registered successfully." });

            return BadRequest(new { Message = "Registration failed.", Errors = errors });
        }
    }
}
