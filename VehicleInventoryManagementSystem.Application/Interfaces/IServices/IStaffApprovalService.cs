using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface IStaffApprovalService
    {
        // Gets all pending customer appointments.
        Task<List<StaffAppointmentApprovalListDto>> GetPendingAppointmentsAsync();

        // Gets all pending customer part requests.
        Task<List<StaffPartRequestApprovalListDto>> GetPendingPartRequestsAsync();

        // Approves customer appointment request.
        Task<string> ApproveAppointmentAsync(int appointmentId);

        // Rejects customer appointment request.
        Task<string> RejectAppointmentAsync(int appointmentId);

        // Marks approved appointment as completed.
        Task<string> CompleteAppointmentAsync(int appointmentId);

        // Approves customer part request.
        Task<string> ApprovePartRequestAsync(int requestId);

        // Rejects customer part request.
        Task<string> RejectPartRequestAsync(int requestId);
    }
}