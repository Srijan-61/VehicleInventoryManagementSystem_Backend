using Microsoft.EntityFrameworkCore;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    public class SalesFeatureService : ISalesFeatureService
    {
        private readonly ISalesFeatureRepository _salesFeatureRepository;
        private readonly AppDbContext _context;

        public SalesFeatureService(
            ISalesFeatureRepository salesFeatureRepository,
            AppDbContext context)
        {
            _salesFeatureRepository = salesFeatureRepository;
            _context = context;
        }

        public async Task<(bool Succeeded, SalesInvoiceResultDto? Data, IEnumerable<string> Errors)>
            CreateSalesInvoiceAsync(CreateSalesInvoiceDto dto)
        {
            var validationErrors = ValidateCreateInvoiceDto(dto);

            if (validationErrors.Any())
                return (false, null, validationErrors);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var customerExists = await _context.Customers
                    .AnyAsync(c => c.Customer_ID == dto.Customer_ID);

                if (!customerExists)
                    return (false, null, new[] { "Customer not found." });

                var staffExists = await _context.StaffProfiles
                    .AnyAsync(s => s.Staff_ID == dto.Staff_ID);

                if (!staffExists)
                    return (false, null, new[] { "Staff profile not found." });

                decimal subTotal = 0;
                var salesItems = new List<SalesItem>();
                var resultItems = new List<SalesItemResultDto>();

                foreach (var item in dto.Items)
                {
                    var part = await _salesFeatureRepository.GetPartByIdAsync(item.Part_ID);

                    if (part == null)
                        return (false, null, new[] { $"Part ID {item.Part_ID} was not found." });

                    if (!part.IsAvailable || part.Stock_Quantity <= 0)
                        return (false, null, new[] { $"{part.Part_Name} is not available." });

                    if (part.Stock_Quantity < item.Quantity)
                        return (false, null, new[] { $"{part.Part_Name} has only {part.Stock_Quantity} item(s) in stock." });

                    decimal itemTotal = part.Unit_Price * item.Quantity;
                    subTotal += itemTotal;

                    salesItems.Add(new SalesItem
                    {
                        Part_ID = part.Part_ID,
                        Quantity_Sold = item.Quantity,
                        Unit_Price = part.Unit_Price,
                        Total_Price = itemTotal
                    });

                    resultItems.Add(new SalesItemResultDto
                    {
                        Part_ID = part.Part_ID,
                        Part_Name = part.Part_Name,
                        Quantity = item.Quantity,
                        Unit_Price = part.Unit_Price,
                        Total_Price = itemTotal
                    });

                    part.Stock_Quantity -= item.Quantity;
                    part.IsAvailable = part.Stock_Quantity > 0;
                    part.Updated_At = DateTime.UtcNow;

                    _salesFeatureRepository.UpdatePart(part);
                }

                if (!salesItems.Any())
                    return (false, null, new[] { "Invoice must contain at least one sales item." });

                decimal discount = 0;
                if (dto.FlatDiscount.HasValue && dto.FlatDiscount.Value > 0)
                {
                    discount = dto.FlatDiscount.Value;
                }
                else if (dto.DiscountPercentage.HasValue && dto.DiscountPercentage.Value > 0)
                {
                    discount = subTotal * (dto.DiscountPercentage.Value / 100m);
                }
                else
                {
                    // 10% loyalty discount when subtotal exceeds 5000 and no explicit discount provided
                    discount = subTotal > 5000 ? subTotal * 0.10m : 0;
                }

                if (discount > subTotal) discount = subTotal;
                if (discount < 0) discount = 0;

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
                    Credit_Due_Date = dto.Is_Paid ? null : DateTime.UtcNow.AddDays(30),
                    Created_At = DateTime.UtcNow
                };

                await _salesFeatureRepository.AddInvoiceAsync(invoice);
                await _salesFeatureRepository.SaveChangesAsync();

                foreach (var salesItem in salesItems)
                {
                    salesItem.Sales_Invoice_No = invoice.Sales_Invoice_No;
                }

                await _salesFeatureRepository.AddSalesItemsAsync(salesItems);
                await _salesFeatureRepository.SaveChangesAsync();

                // Commit the strict Database Transaction if all goes well
                await transaction.CommitAsync();

                var responseData = new SalesInvoiceResultDto
                {
                    Invoice_No = invoice.Sales_Invoice_No,
                    Sub_Total = subTotal,
                    Discount_Amount = discount,
                    Final_Total = finalTotal,
                    Items = resultItems,
                    Message = discount > 0 ? $"Discount of {discount:C} applied!" : "Invoice created successfully."
                };

                return (true, responseData, Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return (false, null, new[]
                {
                    "An unexpected error occurred while creating the sales invoice.",
                    ex.Message
                });
            }
        }

        public async Task<IEnumerable<CustomerDropdownDto>> GetCustomersForDropdownAsync()
        {
            var customers = await _salesFeatureRepository.GetCustomersWithUsersAsync();

            return customers.Select(c => new CustomerDropdownDto
            {
                Customer_ID = c.Customer_ID,
                FullName = c.User.FullName
            });
        }

        public async Task<IEnumerable<PartDropdownDto>> GetPartsForDropdownAsync()
        {
            var parts = await _salesFeatureRepository.GetAvailablePartsAsync();

            return parts.Select(p => new PartDropdownDto
            {
                Part_ID = p.Part_ID,
                Part_Name = p.Part_Name,
                Unit_Price = p.Unit_Price,
                Stock_Quantity = p.Stock_Quantity
            });
        }

        public async Task<int> GetCurrentStaffIdAsync(string userId)
        {
            var staff = await _salesFeatureRepository.GetStaffByUserIdAsync(userId);
            return staff?.Staff_ID ?? 0;
        }

        public async Task<IEnumerable<RecentSalesInvoiceDto>> GetRecentInvoicesAsync(int count = 10)
        {
            var safeCount = count <= 0 ? 10 : count;
            safeCount = safeCount > 50 ? 50 : safeCount;

            var invoices = await _salesFeatureRepository.GetRecentInvoicesAsync(safeCount);

            return invoices.Select(i => new RecentSalesInvoiceDto
            {
                Invoice_No = i.Sales_Invoice_No,
                Sales_Date = i.Sales_Date,
                Customer_Name = i.Customer?.User?.FullName ?? "Unknown Customer",
                Staff_Name = i.Staff?.User?.FullName ?? "Unknown Staff",
                Final_Total = i.Final_Total,
                Sub_Total = i.Sub_Total,
                Discount_Amount = i.Discount_Amount,
                Is_Paid = i.Is_Paid
            });
        }

        private static List<string> ValidateCreateInvoiceDto(CreateSalesInvoiceDto dto)
        {
            var errors = new List<string>();

            if (dto.Customer_ID <= 0)
                errors.Add("Valid customer is required.");

            if (dto.Staff_ID <= 0)
                errors.Add("Valid staff profile is required.");

            if (dto.Items == null || !dto.Items.Any())
            {
                errors.Add("At least one sales item is required.");
                return errors;
            }

            foreach (var item in dto.Items)
            {
                if (item.Part_ID <= 0)
                    errors.Add("Valid part is required.");

                if (item.Quantity <= 0)
                    errors.Add("Quantity must be at least 1.");
            }

            return errors;
        }
    }
}