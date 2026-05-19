using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    [Route("api/Staff/customers")]
    [ApiController]
    [Authorize(Roles = "Staff,Admin")]
    public class CustomerDetailsController : ControllerBase
    {
        private readonly ICustomerDetailsService _service;

        public CustomerDetailsController(ICustomerDetailsService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerDetails(int id)
        {
            var customerDetails = await _service.GetCustomerDetailsAsync(id);

            if (customerDetails == null)
            {
                return NotFound(new { message = "Customer not found." });
            }

            return Ok(customerDetails);
        }
    }
}
