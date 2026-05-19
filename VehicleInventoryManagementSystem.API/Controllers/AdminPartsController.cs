using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using System.Security.Claims;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    [Route("api/admin/parts")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminPartsController : ControllerBase
    {
        private readonly IAdminPartsService _adminPartsService;

        public AdminPartsController(IAdminPartsService adminPartsService)
        {
            _adminPartsService = adminPartsService;
        }

        // Gets all vehicle parts for admin dashboard.
        [HttpGet]
        public async Task<IActionResult> GetAllParts()
        {
            var parts = await _adminPartsService.GetAllPartsAsync();
            return Ok(parts);
        }

        // Creates a purchase invoice and updates stock.
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseParts([FromBody] CreatePurchaseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Invalid admin token." });

            var result = await _adminPartsService.PurchasePartsAsync(dto, userId);

            var successProperty = result.GetType().GetProperty("success");
            var isSuccess = successProperty != null && (bool)successProperty.GetValue(result)!;

            if (!isSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // Creates a brand-new inventory part and records its first purchase in one step.
        [HttpPost("purchase-new")]
        public async Task<IActionResult> PurchaseNewPart([FromBody] CreateNewPartPurchaseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "Invalid admin token." });

            var result = await _adminPartsService.CreateNewPartAndPurchaseAsync(dto, userId);

            var successProperty = result.GetType().GetProperty("success");
            var isSuccess = successProperty != null && (bool)successProperty.GetValue(result)!;

            if (!isSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // Updates part details like name, price, stock, etc.
        [HttpPut("{partId:int}")]
        public async Task<IActionResult> UpdatePart(int partId, [FromBody] UpdateVehiclePartDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminPartsService.UpdatePartAsync(partId, dto);

            if (result.Contains("not found"))
                return NotFound(new { message = result });

            return Ok(new { message = result });
        }

        // Soft deletes part by marking it unavailable.
        [HttpDelete("{partId:int}")]
        public async Task<IActionResult> DeletePart(int partId)
        {
            var result = await _adminPartsService.DeletePartAsync(partId);

            if (result.Contains("not found"))
                return NotFound(new { message = result });

            return Ok(new { message = result });
        }
    }
}