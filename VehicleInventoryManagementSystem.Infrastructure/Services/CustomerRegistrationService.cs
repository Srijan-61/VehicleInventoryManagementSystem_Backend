using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    // This service takes care of registering a new customer and their vehicle 
    // Everything runs inside a transaction so we don't end up with partial data if something goes wrong
 
    public class CustomerRegistrationService : ICustomerRegistrationService
    {
        private readonly UserManager<User> _userManager;
        private readonly ICustomerRegistrationRepository _customerRegistrationRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<CustomerRegistrationService> _logger;
        private readonly IEmailSenderService _emailSenderService;

        public CustomerRegistrationService(
            UserManager<User> userManager,
            ICustomerRegistrationRepository customerRegistrationRepository,
            AppDbContext context,
            ILogger<CustomerRegistrationService> logger,
            IEmailSenderService emailSenderService)
        {
            _userManager = userManager;
            _customerRegistrationRepository = customerRegistrationRepository;
            _context = context;
            _logger = logger;
            _emailSenderService = emailSenderService;
        }

        // Creates a user account, saves customer profile, and adds vehicle info all in one go
        public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterCustomerWithVehicleAsync(RegisterCustomerWithVehicleDto dto)
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

                _logger.LogInformation("Attempting to create user for email: {Email}", dto.Email);
                
                // Auto-generate a secure password if not provided
                var generatedPassword = string.IsNullOrWhiteSpace(dto.Password) 
                    ? $"Cust@{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}!" 
                    : dto.Password;

                var userResult = await _userManager.CreateAsync(user, generatedPassword);
                if (!userResult.Succeeded)
                {
                    _logger.LogWarning("User creation failed for {Email}: {Errors}",
                        dto.Email, string.Join(", ", userResult.Errors.Select(e => e.Description)));
                    return (false, userResult.Errors.Select(e => e.Description));
                }

                _logger.LogInformation("User created, assigning Customer role...");
                var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Failed to assign Customer role: {Errors}",
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    return (false, roleResult.Errors.Select(e => e.Description));
                }

                var customer = new Customer { User_Id = user.Id, Total_Spent = 0, Pending_Credit = 0 };
                await _customerRegistrationRepository.AddCustomerAsync(customer);
                await _customerRegistrationRepository.SaveChangesAsync();

                _logger.LogInformation("Customer record created with ID: {CustomerId}", customer.Customer_ID);

                var vehicle = new Vehicle
                {
                    Customer_ID = customer.Customer_ID,
                    Reg_Number = dto.Reg_Number,
                    Make = dto.Make,
                    Model = dto.Model,
                    Manufacture_Year = dto.Manufacture_Year,
                    Vehicle_Type = dto.Vehicle_Type,
                    Fuel_Type = dto.Fuel_Type,
                    Condition = dto.Condition,
                    Usage_Pattern = dto.Usage_Pattern,
                    Created_At = DateTime.UtcNow
                };

                await _customerRegistrationRepository.AddVehicleAsync(vehicle);
                await _customerRegistrationRepository.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Customer registration completed successfully for {Email}", dto.Email);

                // Send email with credentials
                try
                {
                    var subject = "Welcome to Vehicle Inventory Management System";
                    var body = $@"
                        <div style='font-family: Arial, sans-serif; color: #333;'>
                            <h2>Welcome {dto.FullName}!</h2>
                            <p>Your customer account has been created successfully.</p>
                            <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                                <p style='margin: 5px 0;'><strong>Email:</strong> {dto.Email}</p>
                                <p style='margin: 5px 0;'><strong>Password:</strong> {generatedPassword}</p>
                            </div>
                            <p>Please login and change your password as soon as possible.</p>
                        </div>
                    ";
                    await _emailSenderService.SendEmailAsync(dto.Email, subject, body);
                    _logger.LogInformation("Credentials email sent to {Email}", dto.Email);
                }
                catch (Exception emailEx)
                {
                    _logger.LogWarning(emailEx, "User created but failed to send email to {Email}", dto.Email);
                }

                return (true, Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Customer registration failed with exception for email: {Email}", dto.Email);
                await transaction.RollbackAsync();
                return (false, new List<string> { ex.Message });
            }
        }
    }
}

