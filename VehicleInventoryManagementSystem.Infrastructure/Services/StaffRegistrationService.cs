using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<StaffRegistrationService> _logger;
        private readonly IEmailSenderService _emailSenderService;

        public StaffRegistrationService(
            UserManager<User> userManager,
            IStaffRegistrationRepository staffRegistrationRepository,
            AppDbContext context,
            ILogger<StaffRegistrationService> logger,
            IEmailSenderService emailSenderService)
        {
            _userManager = userManager;
            _staffRegistrationRepository = staffRegistrationRepository;
            _context = context;
            _logger = logger;
            _emailSenderService = emailSenderService;
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

                _logger.LogInformation("Attempting to create staff user for email: {Email}", dto.Email);

                // Auto-generate a secure password for the staff account
                var generatedPassword = $"Staff@{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}!";

                var userResult = await _userManager.CreateAsync(user, generatedPassword);
                if (!userResult.Succeeded)
                {
                    _logger.LogWarning("Staff user creation failed for {Email}: {Errors}",
                        dto.Email, string.Join(", ", userResult.Errors.Select(e => e.Description)));
                    return (false, userResult.Errors.Select(e => e.Description));
                }

                _logger.LogInformation("Staff user created, assigning Staff role...");
                var roleResult = await _userManager.AddToRoleAsync(user, "Staff");
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Failed to assign Staff role: {Errors}",
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    await transaction.RollbackAsync();
                    return (false, roleResult.Errors.Select(e => e.Description));
                }

                var staff = new Staff { User_Id = user.Id };
                await _staffRegistrationRepository.AddStaffAsync(staff);
                await _staffRegistrationRepository.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Staff registration completed successfully for {Email}", dto.Email);

                // Send email with credentials
                try
                {
                    var subject = "Your Staff Account - Vehicle Inventory Management System";
                    var body = $@"
                        <div style='font-family: Arial, sans-serif; color: #333;'>
                            <h2>Welcome {dto.FullName}!</h2>
                            <p>Your staff account has been created successfully by an administrator.</p>
                            <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                                <p style='margin: 5px 0;'><strong>Email:</strong> {dto.Email}</p>
                                <p style='margin: 5px 0;'><strong>Password:</strong> {generatedPassword}</p>
                            </div>
                            <p>Please login and change your password as soon as possible.</p>
                        </div>
                    ";
                    await _emailSenderService.SendEmailAsync(dto.Email, subject, body);
                    _logger.LogInformation("Credentials email sent to staff member {Email}", dto.Email);
                }
                catch (Exception emailEx)
                {
                    _logger.LogWarning(emailEx, "Staff created but failed to send email to {Email}", dto.Email);
                }

                return (true, Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Staff registration failed with exception for email: {Email}", dto.Email);
                await transaction.RollbackAsync();
                return (false, new List<string> { ex.Message });
            }
        }
    }
}
