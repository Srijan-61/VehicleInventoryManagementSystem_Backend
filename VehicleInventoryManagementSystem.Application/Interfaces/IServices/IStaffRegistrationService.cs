using VehicleInventoryManagementSystem.Application.DTOs.Auth;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    // Service interface for staff registration (Feature 2)
    // This defines the contract that our StaffRegistrationService must follow
    public interface IStaffRegistrationService
    {
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterStaffAsync(RegisterStaffDto dto);
    }
}
