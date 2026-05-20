using Microsoft.EntityFrameworkCore;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    public class StaffApprovalRepository : IStaffApprovalRepository
    {
        private readonly AppDbContext _context;

        public StaffApprovalRepository(AppDbContext context)
        {
            _context = context;
        }

        // Gets all customer appointments waiting for staff approval.
        public async Task<List<Appointment>> GetPendingAppointmentsAsync()
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Vehicle)
                .Where(a => a.Appointment_Status == "Pending")
                .OrderBy(a => a.Appointment_Date)
                .ToListAsync();
        }

        // Gets all customer part requests waiting for staff approval.
        public async Task<List<PartRequest>> GetPendingPartRequestsAsync()
        {
            return await _context.PartRequests
                .AsNoTracking()
                .Where(r => r.Status == "Pending")
                .OrderByDescending(r => r.Request_Date)
                .ToListAsync();
        }

        // Gets one appointment for status update.
        public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Vehicle)
                .FirstOrDefaultAsync(a => a.Appointment_ID == appointmentId);
        }

        // Gets one part request for status update.
        public async Task<PartRequest?> GetPartRequestByIdAsync(int requestId)
        {
            return await _context.PartRequests
                .FirstOrDefaultAsync(r => r.Request_ID == requestId);
        }

        // Saves updated status.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}