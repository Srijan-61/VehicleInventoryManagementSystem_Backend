using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    // Service interface for the sales and POS feature (Features 7 and 16)
    // Covers invoice creation, customer lookup for the dropdown, and finding the current staff member
    public interface ISalesFeatureService
    {
        Task<(bool Succeeded, SalesInvoiceResultDto? Data, IEnumerable<string> Errors)> CreateSalesInvoiceAsync(CreateSalesInvoiceDto dto);
        Task<int> GetCurrentStaffIdAsync(string userId);
        Task<IEnumerable<CustomerDropdownDto>> GetCustomersForDropdownAsync();
        Task<IEnumerable<PartDropdownDto>> GetPartsForDropdownAsync();
        Task<IEnumerable<RecentSalesInvoiceDto>> GetRecentInvoicesAsync(int count = 10);
    }
}
