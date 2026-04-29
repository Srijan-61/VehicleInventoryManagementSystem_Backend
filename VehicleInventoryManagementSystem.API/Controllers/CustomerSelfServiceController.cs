using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    // This controller handles all customer self-service features such as booking appointments, requesting parts, and submitting reviews
    [ApiController]
    [Route("api/customer")]
    public class CustomerSelfServiceController : ControllerBase
    {
        // Service layer dependency (business logic)
        private readonly ICustomerSelfService _customerSelfService;

        // Constructor injection
        public CustomerSelfServiceController(ICustomerSelfService customerSelfService)
        {
            _customerSelfService = customerSelfService;
        }

        // BOOK SERVICE APPOINTMENT
        [HttpPost("appointments")]
        public async Task<IActionResult> BookAppointment(CreateAppointmentDto dto)
        {
            // Validate incoming request body
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Call service layer
            var result = await _customerSelfService.BookAppointmentAsync(dto);

            // Return BadRequest only for specific validation errors
            if (result == "Vehicle not found for this customer." ||
                result == "Appointment date must be in the future." ||
                result == "Service type is required.")
            {
                return BadRequest(new { message = result });
            }

            // Success response
            return Ok(new { message = result });
        }

        // REQUEST UNAVAILABLE PART
        
        [HttpPost("part-requests")]
        public async Task<IActionResult> RequestUnavailablePart(CreatePartRequestDto dto)
        {
            // Validate request
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Call service
            var result = await _customerSelfService.RequestUnavailablePartAsync(dto);

            // IMPORTANT:
            // Avoid using Contains("available") because "Unavailable" also contains "available"
            // So we use exact message matching
            if (result == "Customer not found." ||
                result == "Requested part name is required." ||
                result == "Requested quantity must be greater than zero." ||
                result == "This part is currently available in stock.")
            {
                return BadRequest(new { message = result });
            }

            // Success
            return Ok(new { message = result });
        }

        // SUBMIT SERVICE REVIEW
       
        [HttpPost("reviews")]
        public async Task<IActionResult> SubmitReview(CreateReviewDto dto)
        {
            // Validate input
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Call service
            var result = await _customerSelfService.SubmitReviewAsync(dto);

            // Handle validation errors
            if (result == "Appointment not found for this customer." ||
                result == "This appointment has already been reviewed." ||
                result == "Rating must be between 1 and 5." ||
                result == "Review comment is required.")
            {
                return BadRequest(new { message = result });
            }

            // Success response
            return Ok(new { message = result });
        }
    }
}