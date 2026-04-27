using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    public class StaffService : IStaffService
    {
        private readonly UserManager<User> _userManager;
        private readonly IStaffRepository _staffRepository;
        private readonly AppDbContext _context; 

        public StaffService(UserManager<User> userManager, IStaffRepository staffRepository, AppDbContext context)
        {
            _userManager = userManager;
            _staffRepository = staffRepository;
            _context = context;
        }

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

                // 1. Try to create the User
                var userResult = await _userManager.CreateAsync(user, dto.Password);
                if (!userResult.Succeeded)
                {
                    return (false, userResult.Errors.Select(e => e.Description));
                }

                // 2. Try to add the Role
                var roleResult = await _userManager.AddToRoleAsync(user, "Staff");
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return (false, roleResult.Errors.Select(e => e.Description));
                }

                // 3. Create the Profile
                var staff = new Staff { User_Id = user.Id };
                await _staffRepository.AddStaffAsync(staff);
                await _staffRepository.SaveChangesAsync();

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
