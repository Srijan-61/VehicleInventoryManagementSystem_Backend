using VehicleInventoryManagementSystem.Application.DTOs.Customer;

namespace VehicleInventoryManagementSystem.Application.Interfaces.Customer
{
    /// <summary>
    /// Handles customer self-registration (Feature F12).
    /// </summary>
    public interface ICustomerRegistrationService
    {
        Task<RegistrationResultDto> RegisterAsync(
            RegisterCustomerRequest request,
            CancellationToken cancellationToken = default);
    }
}