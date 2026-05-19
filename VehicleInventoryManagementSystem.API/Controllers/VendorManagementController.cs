using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    [Route("api/Admin/vendors")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Ensures only Admins can access
    public class VendorManagementController : ControllerBase
    {
        private readonly IVendorManagementService _service;

        
        public VendorManagementController(IVendorManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vendors = await _service.GetAllVendorsAsync();
            return Ok(vendors);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUpdateVendorDto dto)
        {
            var result = await _service.AddVendorAsync(dto);
            return Ok(new { message = "Vendor added successfully", data = result });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateVendorDto dto)
        {
            var success = await _service.UpdateVendorAsync(id, dto);
            if (!success) return NotFound(new { message = "Vendor not found" });
            
            return Ok(new { message = $"Vendor {id} updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteVendorAsync(id);
            if (!success) return NotFound(new { message = "Vendor not found" });

            return Ok(new { message = $"Vendor {id} deleted successfully" });
        }
    }
}
