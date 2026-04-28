using Microsoft.Extensions.Logging;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    public class SalesService : ISalesService
    {
        private readonly ISalesRepository _salesRepository;
        private readonly ILogger<SalesService> _logger;

        public SalesService(
            ISalesRepository salesRepository,
            ILogger<SalesService> logger)
        {
            _salesRepository = salesRepository;
            _logger = logger;
        }

        public async Task<object> CreateSalesInvoiceAsync(CreateSalesInvoiceDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                return new { success = false, message = "At least one sales item is required." };

            await _salesRepository.BeginTransactionAsync();

            try
            {
                var customer = await _salesRepository.GetCustomerByIdAsync(dto.Customer_ID);

                if (customer == null)
                    return new { success = false, message = "Customer not found." };

                decimal subTotal = 0;
                var salesItems = new List<SalesItem>();

                foreach (var item in dto.Items)
                {
                    var part = await _salesRepository.GetPartByIdAsync(item.Part_ID);

                    if (part == null)
                        return new { success = false, message = $"Part ID {item.Part_ID} not found." };

                    if (part.Stock_Quantity < item.Quantity_Sold)
                        return new { success = false, message = $"Not enough stock for {part.Part_Name}." };

                    var itemTotal = part.Unit_Price * item.Quantity_Sold;

                    subTotal += itemTotal;

                    // Stock is reduced immediately because sale is confirmed.
                    part.Stock_Quantity -= item.Quantity_Sold;
                    part.Updated_At = DateTime.UtcNow;
                    part.IsAvailable = part.Stock_Quantity > 0;

                    salesItems.Add(new SalesItem
                    {
                        Part_ID = part.Part_ID,
                        Quantity_Sold = item.Quantity_Sold,
                        Unit_Price = part.Unit_Price,
                        Total_Price = itemTotal
                    });
                }

                var discountAmount = CalculateLoyaltyDiscount(subTotal);
                var finalTotal = subTotal - discountAmount;

                var invoice = new SalesInvoice
                {
                    Customer_ID = dto.Customer_ID,
                    Staff_ID = dto.Staff_ID,
                    Sales_Date = DateTime.UtcNow,
                    Sub_Total = subTotal,
                    Discount_Amount = discountAmount,
                    Final_Total = finalTotal,
                    Is_Paid = dto.Is_Paid,
                    Credit_Due_Date = dto.Is_Paid ? null : DateTime.UtcNow.AddMonths(1),
                    Created_At = DateTime.UtcNow
                };

                await _salesRepository.AddSalesInvoiceAsync(invoice);
                await _salesRepository.SaveChangesAsync();

                foreach (var item in salesItems)
                {
                    item.Sales_Invoice_No = invoice.Sales_Invoice_No;
                    await _salesRepository.AddSalesItemAsync(item);
                }

                // Customer totals are updated for reports and pending credit tracking.
                customer.Total_Spent += finalTotal;

                if (!dto.Is_Paid)
                {
                    customer.Pending_Credit += finalTotal;
                    customer.Credit_Due_Date = DateTime.UtcNow.AddMonths(1);
                }

                await _salesRepository.SaveChangesAsync();
                await _salesRepository.CommitTransactionAsync();

                _logger.LogInformation("Sales invoice {InvoiceNo} created.", invoice.Sales_Invoice_No);

                return new
                {
                    success = true,
                    message = "Sales invoice created successfully.",
                    invoice.Sales_Invoice_No,
                    invoice.Sub_Total,
                    invoice.Discount_Amount,
                    invoice.Final_Total,
                    Loyalty_Discount_Applied = discountAmount > 0
                };
            }
            catch (Exception ex)
            {
                await _salesRepository.RollbackTransactionAsync();

                _logger.LogError(ex, "Sales invoice creation failed.");

                return new
                {
                    success = false,
                    message = "An error occurred while creating the sales invoice."
                };
            }
        }

        // Applies 10% discount only when a single purchase is above 5000.
        private static decimal CalculateLoyaltyDiscount(decimal subTotal)
        {
            return subTotal > 5000 ? subTotal * 0.10m : 0;
        }
    }
}