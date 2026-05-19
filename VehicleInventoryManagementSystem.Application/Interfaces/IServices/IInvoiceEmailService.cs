using VehicleInventoryManagementSystem.Application.DTOs.Invoices;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface IInvoiceEmailService
    {
        Task<List<CustomerInvoiceDropdownDto>> GetInvoicesByCustomerAsync(int customerId);

        Task<InvoiceEmailDetailsDto?> GetInvoiceEmailDetailsAsync(
            int customerId,
            int salesInvoiceNo
        );

        Task SendInvoiceEmailAsync(SendInvoiceEmailRequestDto request);
    }
}