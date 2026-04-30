using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface ISalesService
    {
        Task<object> CreateSalesInvoiceAsync(CreateSalesInvoiceDto dto);
    }
}