using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    public class CustomerDetailsRepository : ICustomerDetailsRepository
    {
        private readonly AppDbContext _context;

        public CustomerDetailsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(Customer Customer, Vehicle PrimaryVehicle, List<string> ServiceHistory)> GetCustomerDataAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Customer_ID == customerId);

            if (customer == null) return (null, null, null);

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Customer_ID == customerId);

            var history = new List<string>();

            if (vehicle != null)
            {
                history = await _context.Appointments
                    .Where(a => a.Vehicle_ID == vehicle.Vehicle_ID)
                    .Select(a => a.Service_Type)
                    .ToListAsync();
            }

            return (customer, vehicle, history);
        }

        Task<(ICustomerRepository Customer, Vehicle PrimaryVehicle, List<string> ServiceHistory)> ICustomerDetailsRepository.GetCustomerDataAsync(int customerId)
        {
            throw new NotImplementedException();
        }
    }
}
