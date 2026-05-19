using VehicleInventoryManagementSystem.Application.DTOs.Invoices;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface IInvoiceEmailRepository
    {
        Task<List<CustomerInvoiceDropdownDto>> GetInvoicesByCustomerAsync(int customerId);

        Task<InvoiceEmailDetailsDto?> GetInvoiceEmailDetailsAsync(
            int customerId,
            int salesInvoiceNo
        );
    }
}