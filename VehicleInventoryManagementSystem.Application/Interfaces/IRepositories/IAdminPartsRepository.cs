using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface IAdminPartsRepository
    {
        Task<List<VehiclePart>> GetAllPartsAsync();
        Task<VehiclePart?> GetPartByIdAsync(int partId);
        Task<bool> VendorExistsAsync(int vendorId);
        Task<int?> GetAdminIdByUserIdAsync(string userId);

        // Returns true if a part with the same name AND brand already exists.
        // Used to prevent accidental duplicate entries during new-part purchase.
        Task<bool> PartExistsAsync(string partName, string brand);

        // Inserts a brand-new VehiclePart row. Call SaveChangesAsync after to get the Part_ID.
        Task AddPartAsync(VehiclePart part);

        Task AddPurchaseInvoiceAsync(PurchaseInvoice invoice);
        Task AddPurchaseItemAsync(PurchaseItem item);

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        Task SaveChangesAsync();
    }
}