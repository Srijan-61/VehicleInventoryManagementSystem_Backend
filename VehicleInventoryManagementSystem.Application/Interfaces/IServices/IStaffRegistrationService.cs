using VehicleInventoryManagementSystem.Application.DTOs.Auth;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    // Feature 2 staff registration 
    public interface IStaffRegistrationService
    {
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterStaffAsync(RegisterStaffDto dto);
    }
}
