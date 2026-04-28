using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface ICustomerSelfRepository
    {
        Task<Vehicle?> GetCustomerVehicleAsync(int customerId, int vehicleId);
        Task<bool> CustomerExistsAsync(int customerId);
        Task<VehiclePart?> GetPartByNameAsync(string partName);
        Task<Appointment?> GetCustomerAppointmentAsync(int customerId, int appointmentId);
        Task<bool> ReviewExistsForAppointmentAsync(int appointmentId);

        Task AddAppointmentAsync(Appointment appointment);
        Task AddPartRequestAsync(PartRequest partRequest);
        Task AddReviewAsync(Review review);
        Task SaveChangesAsync();
    }
}