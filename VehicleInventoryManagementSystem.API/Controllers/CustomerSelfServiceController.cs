using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/customer")]
    [Authorize(Roles = "Customer")]
    public class CustomerSelfServiceController : ControllerBase
    {
        private readonly ICustomerSelfService _customerSelfService;
        private readonly ILogger<CustomerSelfServiceController> _logger;

        public CustomerSelfServiceController(
            ICustomerSelfService customerSelfService,
            ILogger<CustomerSelfServiceController> logger)
        {
            _customerSelfService = customerSelfService;
            _logger = logger;
        }

        [HttpPost("appointments")]
        public async Task<IActionResult> BookAppointment(
            [FromBody] CreateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetLoggedInUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Invalid customer token." });

            try
            {
                var result = await _customerSelfService
                    .BookAppointmentAsync(dto, userId);

                if (IsValidationError(result))
                    return BadRequest(new { message = result });

                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while booking appointment.");

                return StatusCode(500, new
                {
                    message = "Unable to book appointment. Please try again later."
                });
            }
        }

        [HttpPost("part-requests")]
        public async Task<IActionResult> RequestUnavailablePart(
            [FromBody] CreatePartRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetLoggedInUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Invalid customer token." });

            try
            {
                var result = await _customerSelfService
                    .RequestUnavailablePartAsync(dto, userId);

                if (IsValidationError(result))
                    return BadRequest(new { message = result });

                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while requesting part.");

                return StatusCode(500, new
                {
                    message = "Unable to submit part request. Please try again later."
                });
            }
        }

        [HttpPost("reviews")]
        public async Task<IActionResult> SubmitReview(
            [FromBody] CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetLoggedInUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Invalid customer token." });

            try
            {
                var result = await _customerSelfService
                    .SubmitReviewAsync(dto, userId);

                if (IsValidationError(result))
                    return BadRequest(new { message = result });

                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while submitting review.");

                return StatusCode(500, new
                {
                    message = "Unable to submit review. Please try again later."
                });
            }
        }

        private string? GetLoggedInUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private static bool IsValidationError(string result)
        {
            return result != "Appointment booked successfully." &&
                   result != "Unavailable part request submitted successfully." &&
                   result != "Review submitted successfully.";
        }
    }
}