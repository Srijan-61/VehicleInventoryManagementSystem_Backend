using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    // Features 7 & 16
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
                    var part = await _salesFeatureRepository.GetPartByIdAsync(item.Part_ID);
                    if (part == null || part.Stock_Quantity < item.Quantity)
                        throw new Exception($"Part {item.Part_ID} is unavailable or out of stock.");

                    decimal itemTotal = part.Unit_Price * item.Quantity;
                    subTotal += itemTotal;

                    //add to database entity list
                    salesItems.Add(new SalesItem { Part_ID = part.Part_ID, Quantity_Sold = item.Quantity, Unit_Price = part.Unit_Price, Total_Price = itemTotal });

                    // add to response DTO list (includes Part_Name for the frontend)
                    resultItems.Add(new SalesItemResultDto { Part_ID = part.Part_ID, Part_Name = part.Part_Name, Quantity = item.Quantity, Unit_Price = part.Unit_Price, Total_Price = itemTotal });

                    part.Stock_Quantity -= item.Quantity;
                    _salesFeatureRepository.UpdatePart(part);
                }

                // Feature 16 discout 
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

                await transaction.CommitAsync();

                //build the final response object
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

        // get customers for dropdown
        public async Task<IEnumerable<CustomerDropdownDto>> GetCustomersForDropdownAsync()
        {
            var customers = await _salesFeatureRepository.GetCustomersWithUsersAsync();

            return customers.Select(c => new CustomerDropdownDto
            {
                Customer_ID = c.Customer_ID,
                FullName = c.User.FullName
            });
        }

        // get current staff id
        public async Task<int> GetCurrentStaffIdAsync(string userId)
        {
            var staff = await _salesFeatureRepository.GetStaffByUserIdAsync(userId);
            return staff?.Staff_ID ?? 0;
        }
    }
}
