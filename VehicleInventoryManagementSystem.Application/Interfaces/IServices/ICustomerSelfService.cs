using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface ICustomerSelfService
    {
        Task<string> BookAppointmentAsync(CreateAppointmentDto dto, string userId);
        Task<string> RequestUnavailablePartAsync(CreatePartRequestDto dto, string userId);
        Task<string> SubmitReviewAsync(CreateReviewDto dto, string userId);
    }
}