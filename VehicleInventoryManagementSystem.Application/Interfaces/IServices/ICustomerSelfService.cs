using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface ICustomerSelfService
    {
        // Creates a new appointment for the logged-in customer.
        Task<string> BookAppointmentAsync(CustomerAppointmentDto dto, string userId);

        // Creates a new unavailable part request for the logged-in customer.
        Task<string> RequestUnavailablePartAsync(CustomerPartRequestDto dto, string userId);

        // Submits review for a completed appointment.
        Task<string> SubmitReviewAsync(CustomerReviewDto dto, string userId);

        // Gets vehicles owned by the logged-in customer.
        Task<List<CustomerVehicleListDto>> GetVehiclesAsync(string userId);

        // Gets all appointments of the logged-in customer.
        Task<List<CustomerAppointmentListDto>> GetAppointmentsAsync(string userId);

        // Gets all part requests of the logged-in customer.
        Task<List<CustomerPartRequestListDto>> GetPartRequestsAsync(string userId);

        // Gets completed appointments only for review dropdown.
        Task<List<CustomerAppointmentListDto>> GetCompletedAppointmentsAsync(string userId);

        // Gets reviews submitted by the logged-in customer.
        Task<List<CustomerReviewListDto>> GetReviewsAsync(string userId);
    }
}