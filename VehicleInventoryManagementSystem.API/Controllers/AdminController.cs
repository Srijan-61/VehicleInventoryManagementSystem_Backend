using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.DTOs.Auth;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] 
    public class AdminController : ControllerBase
    {
        private readonly IStaffService _staffService;

        // Constructor Injection for the Service layer
        public AdminController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpPost("register-staff")]
        public async Task<IActionResult> RegisterStaff([FromBody] RegisterStaffDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            
            var (succeeded, errors) = await _staffService.RegisterStaffAsync(dto);

            if (succeeded)
                return Ok(new { Message = "Staff registered successfully." });

          
            return BadRequest(new { Message = "Registration failed.", Errors = errors });
        }
    }
}
