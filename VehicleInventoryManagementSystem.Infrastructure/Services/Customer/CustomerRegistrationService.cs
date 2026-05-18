using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VehicleInventoryManagementSystem.Application.DTOs.Customer;
using VehicleInventoryManagementSystem.Application.Interfaces.Customer;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;
using DomainCustomer = VehicleInventoryManagementSystem.Domain.Models.Customer;

namespace VehicleInventoryManagementSystem.Infrastructure.Services.Customer
{
    /// <summary>
    /// Creates a new Identity User, assigns the "Customer" role, and
    /// creates the linked Customer profile row.
    /// All three operations run in a single transaction so a partial
    /// failure cannot leave orphaned records.
    /// </summary>
    public class CustomerRegistrationService : ICustomerRegistrationService
    {
        private const string CustomerRoleName = "Customer";

        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CustomerRegistrationService> _logger;

        public CustomerRegistrationService(
            UserManager<User> userManager,
            AppDbContext dbContext,
            ILogger<CustomerRegistrationService> logger)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<RegistrationResultDto> RegisterAsync(
            RegisterCustomerRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Reject duplicate emails up front for a clean error message.
            //    Identity would also catch this, but its error wording is generic.
            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing is not null)
            {
                return RegistrationResultDto.Failure(
                    new[] { "An account with this email already exists." });
            }

            // 2. Wrap user + role + profile creation in a transaction.
            await using var transaction = await _dbContext.Database
                .BeginTransactionAsync(cancellationToken);

            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = request.Email,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    FullName = request.FullName,
                    Address = request.Address,
                    Created_At = DateTime.UtcNow
                };

                var createUserResult = await _userManager.CreateAsync(user, request.Password);
                if (!createUserResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return RegistrationResultDto.Failure(
                        createUserResult.Errors.Select(e => e.Description));
                }

                var addToRoleResult = await _userManager.AddToRoleAsync(user, CustomerRoleName);
                if (!addToRoleResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return RegistrationResultDto.Failure(
                        addToRoleResult.Errors.Select(e => e.Description));
                }

                var customerProfile = new DomainCustomer
                {
                    User_Id = user.Id,
                    Pending_Credit = 0m,
                    Total_Spent = 0m,
                    Credit_Due_Date = null
                };

                _dbContext.Customers.Add(customerProfile);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation(
                    "Customer registered: CustomerId={CustomerId}, Email={Email}",
                    customerProfile.Customer_ID, user.Email);

                return RegistrationResultDto.Success(new CustomerResponse
                {
                    CustomerId = customerProfile.Customer_ID,
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber!,
                    Address = user.Address
                });
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Database error during customer registration.");
                return RegistrationResultDto.Failure(
                    new[] { "Could not save customer due to a database error." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Unexpected error during customer registration.");
                return RegistrationResultDto.Failure(
                    new[] { "An unexpected error occurred. Please try again." });
            }
        }
    }
}