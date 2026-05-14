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

        // Gets a vehicle that belongs to the selected customer.
        public async Task<Vehicle?> GetCustomerVehicleAsync(int customerId, int vehicleId)
        {
            return await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Customer_ID == customerId && v.Vehicle_ID == vehicleId);
        }

        // Checks whether the customer exists before creating a request.
        public async Task<bool> CustomerExistsAsync(int customerId)
        {
            return await _context.Customers.AnyAsync(c => c.Customer_ID == customerId);
        }

        // Finds a part by name to avoid requesting available parts.
        public async Task<VehiclePart?> GetPartByNameAsync(string partName)
        {
            return await _context.VehicleParts
                .FirstOrDefaultAsync(p => p.Part_Name.ToLower() == partName.ToLower());
        }

        // Gets appointment only if it belongs to the customer.
        public async Task<Appointment?> GetCustomerAppointmentAsync(int customerId, int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Vehicle)
                .FirstOrDefaultAsync(a =>
                    a.Appointment_ID == appointmentId &&
                    a.Vehicle.Customer_ID == customerId);
        }

        // Prevents duplicate review for the same appointment.
        public async Task<bool> ReviewExistsForAppointmentAsync(int appointmentId)
        {
            return await _context.Reviews.AnyAsync(r => r.Appointment_ID == appointmentId);
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