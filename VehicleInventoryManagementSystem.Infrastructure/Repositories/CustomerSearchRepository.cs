using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    public class CustomerSearchRepository : ICustomerSearchRepository
    {
        private readonly AppDbContext _context;

        public CustomerSearchRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<(Customer Customer, Vehicle PrimaryVehicle)>> SearchCustomersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return new List<(Customer, Vehicle)>();

            searchTerm = searchTerm.ToLower();
            bool isNumericId = int.TryParse(searchTerm, out int parsedId);

            var matchingVehicleCustomerIds = await _context.Vehicles
                .Where(v => v.Reg_Number.ToLower().Contains(searchTerm))
                .Select(v => v.Customer_ID)
                .ToListAsync();

            var matchingCustomers = await _context.Customers
                .Include(c => c.User)
                .Where(c => 
                    (isNumericId && c.Customer_ID == parsedId) || 
                    (c.User.FullName.ToLower().Contains(searchTerm)) || 
                    (c.User.PhoneNumber.Contains(searchTerm)) || 
                    matchingVehicleCustomerIds.Contains(c.Customer_ID)
                )
                .ToListAsync();

            var results = new List<(Customer, Vehicle)>();
            foreach (var customer in matchingCustomers)
            {
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.Customer_ID == customer.Customer_ID);
                
                results.Add((customer, vehicle));
            }

            return results;
        }

        Task<IEnumerable<(ICustomerRepository Customer, Vehicle PrimaryVehicle)>> ICustomerSearchRepository.SearchCustomersAsync(string searchTerm)
        {
            throw new NotImplementedException();
        }
    }
}
