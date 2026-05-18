using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface ICustomerDetailsRepository
    {
        
        Task<(ICustomerRepository Customer, Vehicle PrimaryVehicle, List<string> ServiceHistory)> GetCustomerDataAsync(int customerId);
    }
}
