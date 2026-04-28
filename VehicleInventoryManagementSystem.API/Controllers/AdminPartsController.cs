using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

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

            var result = await _adminPartsService.PurchasePartsAsync(dto);
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