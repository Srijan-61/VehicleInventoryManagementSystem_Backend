using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface ICustomerFeatureService
    {
        Task<CustomerHistoryResultDto> GetCustomerHistoryAsync(string userId);
    }
}
