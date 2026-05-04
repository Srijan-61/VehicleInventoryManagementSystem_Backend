using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
  
    public interface ISalesFeatureRepository
    {
        Task<VehiclePart?> GetPartByIdAsync(int partId);
        void UpdatePart(VehiclePart part);
        Task AddInvoiceAsync(SalesInvoice invoice);
        Task AddSalesItemsAsync(IEnumerable<SalesItem> items);
        Task<Staff?> GetStaffByUserIdAsync(string userId);
        Task<IEnumerable<Customer>> GetCustomersWithUsersAsync();
        Task SaveChangesAsync();
    }
}
