using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    // Feature 2: Staff Registration - Vertical Slice Repository
    // Handles DB transaction to save to AspNetUsers and StaffProfiles
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
