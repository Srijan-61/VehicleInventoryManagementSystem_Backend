using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerSelfServiceController : ControllerBase
    {
        private readonly ICustomerSelfService _customerSelfService;

        public CustomerSelfServiceController(ICustomerSelfService customerSelfService)
        {
            _customerSelfService = customerSelfService;
        }

        [HttpPost("appointments")]
        public async Task<IActionResult> BookAppointment(CreateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _customerSelfService.BookAppointmentAsync(dto);

            if (result.Contains("not found") || result.Contains("must be"))
                return BadRequest(new { message = result });

            return Ok(new { message = result });
        }

        [HttpPost("part-requests")]
        public async Task<IActionResult> RequestUnavailablePart(CreatePartRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _customerSelfService.RequestUnavailablePartAsync(dto);

            if (result.Contains("not found") || result.Contains("available"))
                return BadRequest(new { message = result });

            return Ok(new { message = result });
        }

        [HttpPost("reviews")]
        public async Task<IActionResult> SubmitReview(CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _customerSelfService.SubmitReviewAsync(dto);

            if (result.Contains("not found") || result.Contains("already"))
                return BadRequest(new { message = result });

            return Ok(new { message = result });
        }
    }
}