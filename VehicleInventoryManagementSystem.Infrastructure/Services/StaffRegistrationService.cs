using Microsoft.AspNetCore.Identity;
using VehicleInventoryManagementSystem.Application.DTOs.Auth;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    // This service handles the business logic for registering new staff members (Feature 2)
    // It creates the user account, assigns the "Staff" role, and saves the staff profile all in one transaction
   
    public class StaffRegistrationService : IStaffRegistrationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IStaffRegistrationRepository _staffRegistrationRepository;
        private readonly AppDbContext _context;

        public StaffRegistrationService(
            UserManager<User> userManager,
            IStaffRegistrationRepository staffRegistrationRepository,
            AppDbContext context)
        {
            _userManager = userManager;
            _staffRegistrationRepository = staffRegistrationRepository;
            _context = context;
        }

        // We wrap everything in a transaction so if something fails halfway through, nothing gets saved
        public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterStaffAsync(RegisterStaffDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = dto.Email,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    FullName = dto.FullName,
                    Address = dto.Address,
                    Created_At = DateTime.UtcNow
                };

                var userResult = await _userManager.CreateAsync(user, dto.Password);
                if (!userResult.Succeeded) return (false, userResult.Errors.Select(e => e.Description));

                var roleResult = await _userManager.AddToRoleAsync(user, "Staff");
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return (false, roleResult.Errors.Select(e => e.Description));
                }

                var staff = new Staff { User_Id = user.Id };
                await _staffRegistrationRepository.AddStaffAsync(staff);
                await _staffRegistrationRepository.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, new List<string> { ex.Message });
            }
        }
    }
}
