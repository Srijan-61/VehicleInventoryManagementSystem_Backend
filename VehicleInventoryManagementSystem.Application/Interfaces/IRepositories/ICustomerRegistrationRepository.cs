using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    
    // Repository interface for customer registration (Feature 6)
    // Defines the operations we need to save customer and vehicle data to the database
    public interface ICustomerRegistrationRepository
    {
        Task AddCustomerAsync(ICustomerRepository customer);
        Task AddVehicleAsync(Vehicle vehicle);
        Task SaveChangesAsync();
    }
}
