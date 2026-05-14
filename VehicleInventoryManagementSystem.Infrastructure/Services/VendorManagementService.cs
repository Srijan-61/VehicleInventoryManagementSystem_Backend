using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    public class VendorManagementService : IVendorManagementService
    {
        private readonly IVendorManagementRepository _repository;

        public VendorManagementService(IVendorManagementRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<VendorDto>> GetAllVendorsAsync()
        {
            var vendors = await _repository.GetAllVendorsAsync();
            return vendors.Select(v => new VendorDto
            {
                Id = v.Vendor_ID,
                Name = v.Vendor_Name,
                Phone = v.Vendor_Contact,
                Email = v.Vendor_Email,
                Address = v.Vendor_Address
            });
        }

        public async Task<VendorDto> AddVendorAsync(CreateUpdateVendorDto dto)
        {
            var vendor = new Vendor
            {
                Vendor_Name = dto.Name,
                Vendor_Contact = dto.Phone,
                Vendor_Email = dto.Email,
                Vendor_Address = dto.Address,
                Created_At = System.DateTime.UtcNow
            };

            var savedVendor = await _repository.AddVendorAsync(vendor);

            return new VendorDto
            {
                Id = savedVendor.Vendor_ID,
                Name = savedVendor.Vendor_Name,
                Phone = savedVendor.Vendor_Contact,
                Email = savedVendor.Vendor_Email,
                Address = savedVendor.Vendor_Address
            };
        }

        public async Task<bool> UpdateVendorAsync(int id, CreateUpdateVendorDto dto)
        {
            var vendor = await _repository.GetVendorByIdAsync(id);
            if (vendor == null) return false;

            vendor.Vendor_Name = dto.Name;
            vendor.Vendor_Contact = dto.Phone;
            vendor.Vendor_Email = dto.Email;
            vendor.Vendor_Address = dto.Address;

            await _repository.UpdateVendorAsync(vendor);
            return true;
        }

        public async Task<bool> DeleteVendorAsync(int id)
        {
            var vendor = await _repository.GetVendorByIdAsync(id);
            if (vendor == null) return false;

            await _repository.DeleteVendorAsync(vendor);
            return true;
        }
    }
}
