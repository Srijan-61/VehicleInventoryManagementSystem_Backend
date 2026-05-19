using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface ICustomerSelfRepository
    {
        // Finds Customer_ID using logged-in Identity User ID.
        Task<int?> GetCustomerIdByUserIdAsync(string userId);

        // Checks whether selected vehicle belongs to this customer.
        Task<Vehicle?> GetCustomerVehicleAsync(int customerId, int vehicleId);

        // Checks if the selected appointment time is already booked.
        Task<bool> AppointmentSlotExistsAsync(DateTime appointmentDate);

        // Gets appointment only if it belongs to this customer.
        Task<Appointment?> GetCustomerAppointmentAsync(int customerId, int appointmentId);

        // Checks whether requested part is already available in stock.
        Task<VehiclePart?> GetPartByNameAsync(string partName);

        // Prevents duplicate pending part request.
        Task<bool> ActivePartRequestExistsAsync(int customerId, string partName);

        // Prevents duplicate review for same appointment.
        Task<bool> ReviewExistsForAppointmentAsync(int appointmentId);

        // Gets vehicles for customer appointment dropdown.
        Task<List<Vehicle>> GetCustomerVehiclesAsync(int customerId);

        // Gets customer appointment history.
        Task<List<Appointment>> GetCustomerAppointmentsAsync(int customerId);

        // Gets customer part request history.
        Task<List<PartRequest>> GetCustomerPartRequestsAsync(int customerId);

        // Gets completed appointments only for review.
        Task<List<Appointment>> GetCompletedCustomerAppointmentsAsync(int customerId);

        // Gets customer review history.
        Task<List<Review>> GetCustomerReviewsAsync(int customerId);

        // Saves new appointment.
        Task AddAppointmentAsync(Appointment appointment);

        // Saves new part request.
        Task AddPartRequestAsync(PartRequest partRequest);

        // Saves new review.
        Task AddReviewAsync(Review review);

        // Saves database changes.
        Task SaveChangesAsync();
    }
}