using VehicleInventoryManagementSystem.Application.DTOs.Invoices;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface IInvoiceEmailRepository
    {
        Task<InvoiceEmailDetailsDto?> GetInvoiceEmailDetailsAsync(int salesInvoiceNo);
    }
}