using Microsoft.EntityFrameworkCore;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    public class CustomerSelfRepository : ICustomerSelfRepository
    {
        private readonly AppDbContext _context;

        public CustomerSelfRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int?> GetCustomerIdByUserIdAsync(string userId)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.User_Id == userId);

            return customer?.Customer_ID;
        }

        public async Task<Vehicle?> GetCustomerVehicleAsync(int customerId, int vehicleId)
        {
            return await _context.Vehicles
                .FirstOrDefaultAsync(v =>
                    v.Customer_ID == customerId &&
                    v.Vehicle_ID == vehicleId);
        }

        public async Task<bool> AppointmentSlotExistsAsync(DateTime appointmentDate)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.Appointment_Date == appointmentDate &&
                a.Appointment_Status != "Cancelled");
        }

        public async Task<Appointment?> GetCustomerAppointmentAsync(
            int customerId,
            int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Vehicle)
                .FirstOrDefaultAsync(a =>
                    a.Appointment_ID == appointmentId &&
                    a.Vehicle.Customer_ID == customerId);
        }

        public async Task<VehiclePart?> GetPartByNameAsync(string partName)
        {
            var normalizedPartName = partName.Trim().ToLower();

            return await _context.VehicleParts
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.Part_Name.ToLower() == normalizedPartName);
        }

        public async Task<bool> ActivePartRequestExistsAsync(
            int customerId,
            string partName)
        {
            var normalizedPartName = partName.Trim().ToLower();

            return await _context.PartRequests.AnyAsync(r =>
                r.Customer_ID == customerId &&
                r.Requested_Part_Name.ToLower() == normalizedPartName &&
                r.Status == "Pending");
        }

        public async Task<bool> ReviewExistsForAppointmentAsync(int appointmentId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.Appointment_ID == appointmentId);
        }

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        public async Task AddPartRequestAsync(PartRequest partRequest)
        {
            await _context.PartRequests.AddAsync(partRequest);
        }

        public async Task AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}