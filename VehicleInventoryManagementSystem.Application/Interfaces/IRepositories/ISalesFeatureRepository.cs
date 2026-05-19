using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
  
    // Repository interface for the sales feature (Features 7 and 16)
    // Covers everything from looking up parts and stock to saving invoices and fetching customer/staff data
    public interface ISalesFeatureRepository
    {
        Task<VehiclePart?> GetPartByIdAsync(int partId);
        void UpdatePart(VehiclePart part);
        Task AddInvoiceAsync(SalesInvoice invoice);
        Task AddSalesItemsAsync(IEnumerable<SalesItem> items);
        Task<Staff?> GetStaffByUserIdAsync(string userId);
        Task<IEnumerable<Customer>> GetCustomersWithUsersAsync();
        Task<IEnumerable<VehiclePart>> GetAvailablePartsAsync();
        Task<IEnumerable<SalesInvoice>> GetRecentInvoicesAsync(int count);
        Task SaveChangesAsync();
    }
}
