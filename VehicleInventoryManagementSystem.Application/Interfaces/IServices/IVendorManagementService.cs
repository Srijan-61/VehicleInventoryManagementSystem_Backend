using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    
    

    public interface IVendorManagementService
    {
        Task<IEnumerable<VendorDto>> GetAllVendorsAsync();
        Task<VendorDto> AddVendorAsync(CreateUpdateVendorDto dto);
        Task<bool> UpdateVendorAsync(int id, CreateUpdateVendorDto dto);
        Task<bool> DeleteVendorAsync(int id);
    }
}
