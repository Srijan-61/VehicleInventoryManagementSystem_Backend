using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface IStaffApprovalRepository
    {
        // Gets all appointments waiting for staff approval.
        Task<List<Appointment>> GetPendingAppointmentsAsync();

        // Gets all part requests waiting for staff approval.
        Task<List<PartRequest>> GetPendingPartRequestsAsync();

        // Gets single appointment using appointment ID.
        Task<Appointment?> GetAppointmentByIdAsync(int appointmentId);

        // Gets single part request using request ID.
        Task<PartRequest?> GetPartRequestByIdAsync(int requestId);

        // Saves updated database changes.
        Task SaveChangesAsync();
    }
}