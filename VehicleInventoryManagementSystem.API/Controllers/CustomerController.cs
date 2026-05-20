using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerFeatureService _customerFeatureService;

        public CustomerController(ICustomerFeatureService customerFeatureService)
        {
            _customerFeatureService = customerFeatureService;
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetCustomerHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Invalid token." });
            }

            try
            {
                var result = await _customerFeatureService.GetCustomerHistoryAsync(userId);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
