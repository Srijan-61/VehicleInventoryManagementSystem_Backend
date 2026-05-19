using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface IAdminPartsRepository
    {
        Task<List<VehiclePart>> GetAllPartsAsync();
        Task<VehiclePart?> GetPartByIdAsync(int partId);
        Task<bool> VendorExistsAsync(int vendorId);
        Task<int?> GetAdminIdByUserIdAsync(string userId);

        Task AddPurchaseInvoiceAsync(PurchaseInvoice invoice);
        Task AddPurchaseItemAsync(PurchaseItem item);

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        Task SaveChangesAsync();
    }
}