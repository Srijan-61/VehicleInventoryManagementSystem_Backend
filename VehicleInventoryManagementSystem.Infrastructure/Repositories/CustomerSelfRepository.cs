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

        // Finds customer profile using logged-in Identity user ID.
        public async Task<int?> GetCustomerIdByUserIdAsync(string userId)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.User_Id == userId);

            return customer?.Customer_ID;
        }

        // Gets a vehicle only if it belongs to the logged-in customer.
        public async Task<Vehicle?> GetCustomerVehicleAsync(int customerId, int vehicleId)
        {
            return await _context.Vehicles
                .FirstOrDefaultAsync(v =>
                    v.Customer_ID == customerId &&
                    v.Vehicle_ID == vehicleId);
        }

        // Checks whether appointment slot is already booked.
        public async Task<bool> AppointmentSlotExistsAsync(DateTime appointmentDate)
        {
            return await _context.Appointments
                .AnyAsync(a =>
                    a.Appointment_Date == appointmentDate &&
                    a.Appointment_Status != "Cancelled");
        }

        // Gets appointment only if it belongs to the logged-in customer.
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

        // Finds part by name to check whether it is already available.
        public async Task<VehiclePart?> GetPartByNameAsync(string partName)
        {
            var normalizedPartName = partName.Trim().ToLower();

            return await _context.VehicleParts
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.Part_Name.ToLower() == normalizedPartName);
        }

        // Checks duplicate pending part request for same customer and same part.
        public async Task<bool> ActivePartRequestExistsAsync(
            int customerId,
            string partName)
        {
            var normalizedPartName = partName.Trim().ToLower();

            return await _context.PartRequests
                .AnyAsync(r =>
                    r.Customer_ID == customerId &&
                    r.Requested_Part_Name.ToLower() == normalizedPartName &&
                    r.Status == "Pending");
        }

        // Checks whether this appointment already has a review.
        public async Task<bool> ReviewExistsForAppointmentAsync(int appointmentId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.Appointment_ID == appointmentId);
        }

        // Gets all vehicles owned by the logged-in customer.
        public async Task<List<Vehicle>> GetCustomerVehiclesAsync(int customerId)
        {
            return await _context.Vehicles
                .AsNoTracking()
                .Where(v => v.Customer_ID == customerId)
                .OrderBy(v => v.Reg_Number)
                .ToListAsync();
        }

        // Gets all appointment history of the logged-in customer.
        public async Task<List<Appointment>> GetCustomerAppointmentsAsync(int customerId)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Vehicle)
                .Where(a => a.Vehicle.Customer_ID == customerId)
                .OrderByDescending(a => a.Appointment_Date)
                .ToListAsync();
        }

        // Gets all unavailable part requests of the logged-in customer.
        public async Task<List<PartRequest>> GetCustomerPartRequestsAsync(int customerId)
        {
            return await _context.PartRequests
                .AsNoTracking()
                .Where(r => r.Customer_ID == customerId)
                .OrderByDescending(r => r.Request_Date)
                .ToListAsync();
        }

        // Gets completed appointments that customer can review.
        public async Task<List<Appointment>> GetCompletedCustomerAppointmentsAsync(int customerId)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Vehicle)
                .Where(a =>
                    a.Vehicle.Customer_ID == customerId &&
                    a.Appointment_Status == "Completed")
                .OrderByDescending(a => a.Appointment_Date)
                .ToListAsync();
        }

        // Gets all reviews submitted by the logged-in customer.
        public async Task<List<Review>> GetCustomerReviewsAsync(int customerId)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Include(r => r.Appointment)
                .Where(r => r.Customer_ID == customerId)
                .OrderByDescending(r => r.Review_Date)
                .ToListAsync();
        }

        // Adds a new appointment record.
        public async Task AddAppointmentAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        // Adds a new unavailable part request.
        public async Task AddPartRequestAsync(PartRequest partRequest)
        {
            await _context.PartRequests.AddAsync(partRequest);
        }

        // Adds a new service review.
        public async Task AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        // Saves all pending database changes.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}