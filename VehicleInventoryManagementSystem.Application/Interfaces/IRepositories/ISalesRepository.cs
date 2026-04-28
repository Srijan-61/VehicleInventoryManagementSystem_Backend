using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface ISalesRepository
    {
        Task<Customer?> GetCustomerByIdAsync(int customerId);
        Task<VehiclePart?> GetPartByIdAsync(int partId);

        Task AddSalesInvoiceAsync(SalesInvoice invoice);
        Task AddSalesItemAsync(SalesItem item);

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        Task SaveChangesAsync();
    }
}