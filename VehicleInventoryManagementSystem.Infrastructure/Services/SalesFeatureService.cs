using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    // This service contains all the business logic for sales and POS (Features 7 and 16)
    // It handles creating invoices, applying discounts, updating stock, and fetching helper data
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

        // Main method that creates a sales invoice - it loops through each item, checks stock,
        // calculates totals, applies the loyalty discount if applicable, and saves everything
        public async Task<(bool Succeeded, SalesInvoiceResultDto? Data, IEnumerable<string> Errors)> CreateSalesInvoiceAsync(CreateSalesInvoiceDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal subTotal = 0;
                var salesItems = new List<SalesItem>();
                var resultItems = new List<SalesItemResultDto>(); // this will hold the item details we send back to the frontend

                foreach (var item in dto.Items)
                {
                    var part = await _salesFeatureRepository.GetPartByIdAsync(item.Part_ID);
                    if (part == null || part.Stock_Quantity < item.Quantity)
                        throw new Exception($"Part {item.Part_ID} is unavailable or out of stock.");

                    decimal itemTotal = part.Unit_Price * item.Quantity;
                    subTotal += itemTotal;

                    // save this item for the database
                    salesItems.Add(new SalesItem { Part_ID = part.Part_ID, Quantity_Sold = item.Quantity, Unit_Price = part.Unit_Price, Total_Price = itemTotal });

                    // also save it for the response so the frontend can display the part name
                    resultItems.Add(new SalesItemResultDto { Part_ID = part.Part_ID, Part_Name = part.Part_Name, Quantity = item.Quantity, Unit_Price = part.Unit_Price, Total_Price = itemTotal });

                    part.Stock_Quantity -= item.Quantity;
                    _salesFeatureRepository.UpdatePart(part);
                }

                // Feature 16: if the subtotal is over 5000, we give a 10% loyalty discount
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

                await _salesFeatureRepository.AddInvoiceAsync(invoice);
                await _salesFeatureRepository.SaveChangesAsync();

                foreach (var si in salesItems) si.Sales_Invoice_No = invoice.Sales_Invoice_No;
                await _salesFeatureRepository.AddSalesItemsAsync(salesItems);
                await _salesFeatureRepository.SaveChangesAsync();

                await transaction.CommitAsync();

                // put together the response with all the invoice details
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

        // Returns all customers with their names for the dropdown on the sales form
        public async Task<IEnumerable<CustomerDropdownDto>> GetCustomersForDropdownAsync()
        {
            var customers = await _salesFeatureRepository.GetCustomersWithUsersAsync();

            return customers.Select(c => new CustomerDropdownDto
            {
                Customer_ID = c.Customer_ID,
                FullName = c.User.FullName
            });
        }

        // Finds the Staff_ID for the currently logged-in user so we can link the invoice to them
        public async Task<int> GetCurrentStaffIdAsync(string userId)
        {
            var staff = await _salesFeatureRepository.GetStaffByUserIdAsync(userId);
            return staff?.Staff_ID ?? 0;
        }
    }
}
