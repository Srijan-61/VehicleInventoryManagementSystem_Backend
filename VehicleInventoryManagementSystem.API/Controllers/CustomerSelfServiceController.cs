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

        // Books an appointment for the logged-in customer.
        [HttpPost("appointments")]
        public async Task<IActionResult> BookAppointment(
            [FromBody] CustomerAppointmentDto dto)
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

        // Creates an unavailable part request for the logged-in customer.
        [HttpPost("part-requests")]
        public async Task<IActionResult> RequestUnavailablePart(
            [FromBody] CustomerPartRequestDto dto)
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

        // Submits a review for completed appointment.
        [HttpPost("reviews")]
        public async Task<IActionResult> SubmitReview(
            [FromBody] CustomerReviewDto dto)
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

        // Gets vehicles owned by the logged-in customer.
        [HttpGet("vehicles")]
        public async Task<IActionResult> GetVehicles()
        {
            var userId = GetLoggedInUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Invalid customer token." });

            var vehicles = await _customerSelfService.GetVehiclesAsync(userId);
            return Ok(vehicles);
        }

        // Gets all appointments of the logged-in customer.
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments()
        {
            var userId = GetLoggedInUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Invalid customer token." });

            var appointments = await _customerSelfService.GetAppointmentsAsync(userId);
            return Ok(appointments);
        }

        // Gets all unavailable part requests of the logged-in customer.
        [HttpGet("part-requests")]
        public async Task<IActionResult> GetPartRequests()
        {
            var userId = GetLoggedInUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Invalid customer token." });

            var requests = await _customerSelfService.GetPartRequestsAsync(userId);
            return Ok(requests);
        }

        // Gets completed appointments for review dropdown.
        [HttpGet("appointments/completed")]
        public async Task<IActionResult> GetCompletedAppointments()
        {
            var userId = GetLoggedInUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Invalid customer token." });

            var appointments = await _customerSelfService
                .GetCompletedAppointmentsAsync(userId);

            return Ok(appointments);
        }

        // Gets reviews submitted by the logged-in customer.
        [HttpGet("reviews")]
        public async Task<IActionResult> GetReviews()
        {
            var userId = GetLoggedInUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Invalid customer token." });

            var reviews = await _customerSelfService.GetReviewsAsync(userId);
            return Ok(reviews);
        }

        // Reads logged-in user's ID from JWT token.
        private string? GetLoggedInUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // Converts service validation messages into BadRequest responses.
        private static bool IsValidationError(string result)
        {
            return result != "Appointment booked successfully." &&
                   result != "Unavailable part request submitted successfully." &&
                   result != "Review submitted successfully.";
        }
    }
}