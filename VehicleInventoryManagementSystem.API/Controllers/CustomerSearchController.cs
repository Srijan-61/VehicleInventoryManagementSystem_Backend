using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    [Route("api/Staff/search-customers")]
    [ApiController]
    [Authorize(Roles = "Staff")]
    public class CustomerSearchController : ControllerBase
    {
        private readonly ICustomerSearchService _service;

        public CustomerSearchController(ICustomerSearchService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "Search term cannot be empty." });
            }

            var results = await _service.SearchAsync(query);
            return Ok(results);
        }
    }
}
