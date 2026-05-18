using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    // This repository handles saving customer and vehicle records to the database (Feature 6)
    // It manages the Customers and Vehicles tables during the registration process
    public class CustomerRegistrationRepository : ICustomerRegistrationRepository
    {
        private readonly AppDbContext _context;

        public CustomerRegistrationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public Task AddCustomerAsync(ICustomerRepository customer)
        {
            throw new NotImplementedException();
        }

        public async Task AddVehicleAsync(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
