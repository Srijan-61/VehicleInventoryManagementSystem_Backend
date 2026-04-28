using System;
using System.Collections.Generic;
using System.Text;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;
        public CustomerRepository(AppDbContext context) => _context = context;

        public async Task AddCustomerAsync(Customer customer) => await _context.Customers.AddAsync(customer);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
