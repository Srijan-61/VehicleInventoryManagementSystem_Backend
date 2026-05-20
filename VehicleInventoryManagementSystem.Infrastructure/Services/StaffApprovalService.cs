using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    public class StaffApprovalService : IStaffApprovalService
    {
        private readonly IStaffApprovalRepository _repository;

        public StaffApprovalService(IStaffApprovalRepository repository)
        {
            _repository = repository;
        }

        // Shows pending appointments to staff.
        public async Task<List<StaffAppointmentApprovalListDto>> GetPendingAppointmentsAsync()
        {
            var appointments = await _repository.GetPendingAppointmentsAsync();

            return appointments.Select(a => new StaffAppointmentApprovalListDto
            {
                Appointment_ID = a.Appointment_ID,
                Vehicle_ID = a.Vehicle_ID,
                Customer_ID = a.Vehicle.Customer_ID,
                VehicleName = $"{a.Vehicle.Make} {a.Vehicle.Model} ({a.Vehicle.Reg_Number})",
                Appointment_Date = a.Appointment_Date,
                Service_Type = a.Service_Type,
                Appointment_Status = a.Appointment_Status
            }).ToList();
        }

        // Shows pending part requests to staff.
        public async Task<List<StaffPartRequestApprovalListDto>> GetPendingPartRequestsAsync()
        {
            var requests = await _repository.GetPendingPartRequestsAsync();

            return requests.Select(r => new StaffPartRequestApprovalListDto
            {
                Request_ID = r.Request_ID,
                Customer_ID = r.Customer_ID,
                Requested_Part_Name = r.Requested_Part_Name,
                Requested_Quantity = r.Requested_Quantity,
                Status = r.Status,
                Request_Date = r.Request_Date
            }).ToList();
        }

        // Staff approves pending appointment.
        public async Task<string> ApproveAppointmentAsync(int appointmentId)
        {
            var appointment = await _repository.GetAppointmentByIdAsync(appointmentId);

            if (appointment == null)
                return "Appointment not found.";

            if (appointment.Appointment_Status != "Pending")
                return "Only pending appointments can be approved.";

            appointment.Appointment_Status = "Approved";

            await _repository.SaveChangesAsync();

            return "Appointment approved successfully.";
        }

        // Staff rejects pending appointment.
        public async Task<string> RejectAppointmentAsync(int appointmentId)
        {
            var appointment = await _repository.GetAppointmentByIdAsync(appointmentId);

            if (appointment == null)
                return "Appointment not found.";

            if (appointment.Appointment_Status != "Pending")
                return "Only pending appointments can be rejected.";

            appointment.Appointment_Status = "Rejected";

            await _repository.SaveChangesAsync();

            return "Appointment rejected successfully.";
        }

        // Staff marks approved appointment as completed.
        // After this, customer can leave review.
        public async Task<string> CompleteAppointmentAsync(int appointmentId)
        {
            var appointment = await _repository.GetAppointmentByIdAsync(appointmentId);

            if (appointment == null)
                return "Appointment not found.";

            if (appointment.Appointment_Status != "Approved")
                return "Only approved appointments can be completed.";

            appointment.Appointment_Status = "Completed";

            await _repository.SaveChangesAsync();

            return "Appointment completed successfully.";
        }

        // Staff approves pending part request.
        public async Task<string> ApprovePartRequestAsync(int requestId)
        {
            var request = await _repository.GetPartRequestByIdAsync(requestId);

            if (request == null)
                return "Part request not found.";

            if (request.Status != "Pending")
                return "Only pending part requests can be approved.";

            request.Status = "Approved";

            await _repository.SaveChangesAsync();

            return "Part request approved successfully.";
        }

        // Staff rejects pending part request.
        public async Task<string> RejectPartRequestAsync(int requestId)
        {
            var request = await _repository.GetPartRequestByIdAsync(requestId);

            if (request == null)
                return "Part request not found.";

            if (request.Status != "Pending")
                return "Only pending part requests can be rejected.";

            request.Status = "Rejected";

            await _repository.SaveChangesAsync();

            return "Part request rejected successfully.";
        }
    }
}