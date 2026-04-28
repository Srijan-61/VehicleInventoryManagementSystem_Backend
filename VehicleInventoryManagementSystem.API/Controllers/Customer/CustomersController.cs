using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.DTOs.Customer;
using VehicleInventoryManagementSystem.Application.Interfaces.Customer;

namespace VehicleInventoryManagementSystem.API.Controllers.Customer
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRegistrationService _registrationService;

        public CustomersController(ICustomerRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        /// <summary>
        /// Customer self-registration (Feature F12).
        /// </summary>
        /// <response code="201">Customer created successfully.</response>
        /// <response code="400">Validation or business rule failure.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterCustomerRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _registrationService.RegisterAsync(request, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return CreatedAtAction(
                nameof(Register),
                new { id = result.Customer!.CustomerId },
                result.Customer);
        }
    }
}