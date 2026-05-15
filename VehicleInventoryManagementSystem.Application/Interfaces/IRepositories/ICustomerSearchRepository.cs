using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface ICustomerSearchRepository
    {
        Task<IEnumerable<(ICustomerRepository Customer, Vehicle PrimaryVehicle)>> SearchCustomersAsync(string searchTerm);
    }
}
