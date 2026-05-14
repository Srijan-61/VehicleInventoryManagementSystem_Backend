using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    // This repository is responsible for saving staff data to the database (Feature 2)
    // It works with the StaffProfiles table to persist new staff records
    public class StaffRegistrationRepository : IStaffRegistrationRepository
    {
        private readonly AppDbContext _context;

        public StaffRegistrationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddStaffAsync(Staff staff)
        {
            await _context.StaffProfiles.AddAsync(staff);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
