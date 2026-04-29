using System;
using System.Collections.Generic;
using System.Text;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AppDbContext _context;
        public VehicleRepository(AppDbContext context) => _context = context;

        public async Task AddVehicleAsync(Vehicle vehicle) => await _context.Vehicles.AddAsync(vehicle);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
