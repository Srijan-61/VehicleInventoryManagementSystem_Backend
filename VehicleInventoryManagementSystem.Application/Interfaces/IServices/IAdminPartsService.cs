using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface IAdminPartsService
    {
        Task<List<VehiclePart>> GetAllPartsAsync();
        Task<object> PurchasePartsAsync(CreatePurchaseDto dto, string userId);
        Task<object> CreateNewPartAndPurchaseAsync(CreateNewPartPurchaseDto dto, string userId);
        Task<string> UpdatePartAsync(int partId, UpdateVehiclePartDto dto);
        Task<string> DeletePartAsync(int partId);
    }
}