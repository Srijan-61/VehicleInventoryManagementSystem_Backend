using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    
    public interface ICustomerRegistrationRepository
    {
        Task AddCustomerAsync(Customer customer);
        Task AddVehicleAsync(Vehicle vehicle);
        Task SaveChangesAsync();
    }
}
