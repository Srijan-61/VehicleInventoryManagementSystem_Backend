using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    // Service interface for customer registration (Feature 6)
    // The service layer handles creating the user account along with their vehicle details
    public interface ICustomerRegistrationService
    {
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterCustomerWithVehicleAsync(RegisterCustomerWithVehicleDto dto);
    }
}
