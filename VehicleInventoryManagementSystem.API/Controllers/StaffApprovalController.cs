using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/staff/approvals")]
    [Authorize(Roles = "Staff,Admin")]
    public class StaffApprovalController : ControllerBase
    {
        private readonly IStaffApprovalService _service;

        public StaffApprovalController(IStaffApprovalService service)
        {
            _service = service;
        }

        // Gets all pending customer appointments.
        [HttpGet("appointments/pending")]
        public async Task<IActionResult> GetPendingAppointments()
        {
            var data = await _service.GetPendingAppointmentsAsync();

            return Ok(data);
        }

        // Approves customer appointment request.
        [HttpPut("appointments/{appointmentId}/approve")]
        public async Task<IActionResult> ApproveAppointment(int appointmentId)
        {
            var message = await _service.ApproveAppointmentAsync(appointmentId);

            return Ok(new { message });
        }

        // Rejects customer appointment request.
        [HttpPut("appointments/{appointmentId}/reject")]
        public async Task<IActionResult> RejectAppointment(int appointmentId)
        {
            var message = await _service.RejectAppointmentAsync(appointmentId);

            return Ok(new { message });
        }

        // Marks approved appointment as completed.
        [HttpPut("appointments/{appointmentId}/complete")]
        public async Task<IActionResult> CompleteAppointment(int appointmentId)
        {
            var message = await _service.CompleteAppointmentAsync(appointmentId);

            return Ok(new { message });
        }

        // Gets all pending part requests.
        [HttpGet("parts/pending")]
        public async Task<IActionResult> GetPendingPartRequests()
        {
            var data = await _service.GetPendingPartRequestsAsync();

            return Ok(data);
        }

        // Approves customer part request.
        [HttpPut("parts/{requestId}/approve")]
        public async Task<IActionResult> ApprovePartRequest(int requestId)
        {
            var message = await _service.ApprovePartRequestAsync(requestId);

            return Ok(new { message });
        }

        // Rejects customer part request.
        [HttpPut("parts/{requestId}/reject")]
        public async Task<IActionResult> RejectPartRequest(int requestId)
        {
            var message = await _service.RejectPartRequestAsync(requestId);

            return Ok(new { message });
        }
    }
}