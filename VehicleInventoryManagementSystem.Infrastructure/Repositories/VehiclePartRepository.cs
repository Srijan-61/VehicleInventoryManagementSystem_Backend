using System;
using System.Collections.Generic;
using System.Text;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    public class VehiclePartRepository(AppDbContext _context) : IVehiclePartRepository
    {
        public async Task<VehiclePart?> GetByIdAsync(int partId) => await _context.VehicleParts.FindAsync(partId);
        public void Update(VehiclePart part) => _context.VehicleParts.Update(part);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
