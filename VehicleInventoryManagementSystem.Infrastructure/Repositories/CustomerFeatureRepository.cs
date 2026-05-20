using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    public class CustomerFeatureRepository : ICustomerFeatureRepository
    {
        private readonly AppDbContext _context;

        public CustomerFeatureRepository(AppDbContext context)
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

        public async Task<List<CustomerHistoryItemDto>> GetCustomerHistoryAsync(int customerId)
        {
            // 1. Fetch Sales Invoices for this customer
            var invoicesQuery = _context.SalesInvoices
                .AsNoTracking()
                .Where(s => s.Customer_ID == customerId)
                .Select(s => new CustomerHistoryItemDto
                {
                    Date = s.Sales_Date,
                    Type = "Purchase",
                    Description = $"Invoice #{s.Sales_Invoice_No}",
                    TotalAmount = s.Final_Total,
                    Status = s.Is_Paid ? "Paid" : "Pending"
                });

            // 2. Fetch Service Appointments for this customer (via Vehicle)
            var appointmentsQuery = _context.Appointments
                .AsNoTracking()
                .Where(a => a.Vehicle.Customer_ID == customerId)
                .Select(a => new CustomerHistoryItemDto
                {
                    Date = a.Appointment_Date,
                    Type = "Service",
                    Description = $"{a.Service_Type} - {a.Vehicle.Make} {a.Vehicle.Model}",
                    TotalAmount = null, // Appointments don't have a final cost in the current schema
                    Status = a.Appointment_Status
                });

            // 3. Execute both queries and merge in memory, ordering by date descending
            var invoices = await invoicesQuery.ToListAsync();
            var appointments = await appointmentsQuery.ToListAsync();

            var combinedHistory = invoices.Concat(appointments)
                .OrderByDescending(h => h.Date)
                .ToList();

            return combinedHistory;
        }
    }
}
