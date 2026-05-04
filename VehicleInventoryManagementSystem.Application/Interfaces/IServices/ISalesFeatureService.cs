using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    // features 7 & 16 
    public interface ISalesFeatureService
    {
        Task<(bool Succeeded, SalesInvoiceResultDto? Data, IEnumerable<string> Errors)> CreateSalesInvoiceAsync(CreateSalesInvoiceDto dto);
        Task<int> GetCurrentStaffIdAsync(string userId);
        Task<IEnumerable<CustomerDropdownDto>> GetCustomersForDropdownAsync();
    }
}
