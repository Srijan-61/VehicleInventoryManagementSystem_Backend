using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.DTOs.Auth;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    // Primary Constructor ,dependencies directly into private fields
    public class StaffService(
        UserManager<User> _userManager,
        IStaffRepository _staffRepository,
        ICustomerRepository _customerRepository,
        IVehicleRepository _vehicleRepository,
        ISalesRepository _salesRepository,
        IVehiclePartRepository _partRepository,
        AppDbContext _context) : IStaffService
    {

        // Registers a new staff member and assigns the appropriate role
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
                await _customerRepository.AddCustomerAsync(customer);
                await _customerRepository.SaveChangesAsync();

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

                await _vehicleRepository.AddVehicleAsync(vehicle);
                await _vehicleRepository.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, new List<string> { ex.Message });
            }
        }

        // Creates a sales invoice, updates part stock, and applies loyalty discounts if applicable
        public async Task<(bool Succeeded, SalesInvoiceResultDto? Data, IEnumerable<string> Errors)> CreateSalesInvoiceAsync(CreateSalesInvoiceDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal subTotal = 0;
                var salesItems = new List<SalesItem>();
                var resultItems = new List<SalesItemResultDto>(); // For the response

                foreach (var item in dto.Items)
                {
                    var part = await _partRepository.GetByIdAsync(item.Part_ID);
                    if (part == null || part.Stock_Quantity < item.Quantity)
                        throw new Exception($"Part {item.Part_ID} is unavailable or out of stock.");

                    decimal itemTotal = part.Unit_Price * item.Quantity;
                    subTotal += itemTotal;

                    // 1. Add to database entity list
                    salesItems.Add(new SalesItem { Part_ID = part.Part_ID, Quantity_Sold = item.Quantity, Unit_Price = part.Unit_Price, Total_Price = itemTotal });

                    // 2. Add to response DTO list (includes Part_Name for the frontend)
                    resultItems.Add(new SalesItemResultDto { Part_ID = part.Part_ID, Part_Name = part.Part_Name, Quantity = item.Quantity, Unit_Price = part.Unit_Price, Total_Price = itemTotal });

                    part.Stock_Quantity -= item.Quantity;
                    _partRepository.Update(part);
                }

                decimal discount = subTotal > 5000 ? subTotal * 0.10m : 0;
                decimal finalTotal = subTotal - discount;

                var invoice = new SalesInvoice
                {
                    Customer_ID = dto.Customer_ID,
                    Staff_ID = dto.Staff_ID,
                    Sales_Date = DateTime.UtcNow,
                    Sub_Total = subTotal,
                    Discount_Amount = discount,
                    Final_Total = finalTotal,
                    Is_Paid = dto.Is_Paid,
                    Created_At = DateTime.UtcNow
                };

                await _salesRepository.AddInvoiceAsync(invoice);
                await _salesRepository.SaveChangesAsync();

                foreach (var si in salesItems) si.Sales_Invoice_No = invoice.Sales_Invoice_No;
                await _salesRepository.AddSalesItemsAsync(salesItems);

                await transaction.CommitAsync();

                // 3. Build the final response object
                var responseData = new SalesInvoiceResultDto
                {
                    Invoice_No = invoice.Sales_Invoice_No,
                    Sub_Total = subTotal,
                    Discount_Amount = discount,
                    Final_Total = finalTotal,
                    Items = resultItems,
                    Message = discount > 0 ? "10% Loyalty discount applied!" : "Invoice created successfully."
                };

                return (true, responseData, Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, null, new[] { ex.Message });
            }
        }

        // Fetch Customers for Dropdown 
        public async Task<IEnumerable<CustomerDropdownDto>> GetCustomersForDropdownAsync()
        {
            var customers = await _customerRepository.GetCustomersWithUsersAsync();

            return customers.Select(c => new CustomerDropdownDto
            {
                Customer_ID = c.Customer_ID,
                FullName = c.User.FullName
            });
        }

        // Fetch Staff ID 
        public async Task<int> GetCurrentStaffIdAsync(string userId)
        {
            var staff = await _staffRepository.GetStaffByUserIdAsync(userId);
            return staff?.Staff_ID ?? 0;
        }
    }
}