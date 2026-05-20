using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface ICustomerFeatureRepository
    {
        Task<int?> GetCustomerIdByUserIdAsync(string userId);
        Task<List<CustomerHistoryItemDto>> GetCustomerHistoryAsync(int customerId);
    }
}
