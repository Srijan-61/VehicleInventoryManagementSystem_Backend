using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface IAdminPartsRepository
    {
        Task<List<VehiclePart>> GetAllPartsAsync();
        Task<VehiclePart?> GetPartByIdAsync(int partId);
        Task<bool> VendorExistsAsync(int vendorId);
        Task<bool> AdminExistsAsync(int adminId);

        // Ensure PurchaseInvoice and PurchaseItem are types, not namespaces
        Task AddPurchaseInvoiceAsync(VehicleInventoryManagementSystem.Domain.Models.PurchaseInvoice invoice);
        Task AddPurchaseItemAsync(VehicleInventoryManagementSystem.Domain.Models.PurchaseItem item);

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        Task SaveChangesAsync();
    }
}