using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface ICustomerSelfService
    {
        Task<string> BookAppointmentAsync(CreateAppointmentDto dto);
        Task<string> RequestUnavailablePartAsync(CreatePartRequestDto dto);
        Task<string> SubmitReviewAsync(CreateReviewDto dto);
    }
}