using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface ICustomerSelfRepository
    {
        Task<int?> GetCustomerIdByUserIdAsync(string userId);

        Task<Vehicle?> GetCustomerVehicleAsync(int customerId, int vehicleId);
        Task<bool> AppointmentSlotExistsAsync(DateTime appointmentDate);
        Task<Appointment?> GetCustomerAppointmentAsync(int customerId, int appointmentId);

        Task<VehiclePart?> GetPartByNameAsync(string partName);
        Task<bool> ActivePartRequestExistsAsync(int customerId, string partName);

        Task<bool> ReviewExistsForAppointmentAsync(int appointmentId);

        Task AddAppointmentAsync(Appointment appointment);
        Task AddPartRequestAsync(PartRequest partRequest);
        Task AddReviewAsync(Review review);

        Task SaveChangesAsync();
    }
}