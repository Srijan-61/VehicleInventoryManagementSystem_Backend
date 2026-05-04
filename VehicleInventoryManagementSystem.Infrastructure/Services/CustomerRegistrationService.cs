using Microsoft.AspNetCore.Identity;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    // feature 6: customer registration 
 
    public class CustomerRegistrationService : ICustomerRegistrationService
    {
        private readonly UserManager<User> _userManager;
        private readonly ICustomerRegistrationRepository _customerRegistrationRepository;
        private readonly AppDbContext _context;

        public CustomerRegistrationService(
            UserManager<User> userManager,
            ICustomerRegistrationRepository customerRegistrationRepository,
            AppDbContext context)
        {
            _userManager = userManager;
            _customerRegistrationRepository = customerRegistrationRepository;
            _context = context;
        }

        // Registers a new customer and adds their initial vehicle details
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

                var userResult = await _userManager.CreateAsync(user, dto.Password);
                if (!userResult.Succeeded) return (false, userResult.Errors.Select(e => e.Description));

                await _userManager.AddToRoleAsync(user, "Customer");

                var customer = new Customer { User_Id = user.Id, Total_Spent = 0, Pending_Credit = 0 };
                await _customerRegistrationRepository.AddCustomerAsync(customer);
                await _customerRegistrationRepository.SaveChangesAsync();

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
