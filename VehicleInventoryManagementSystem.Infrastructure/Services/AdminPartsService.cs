using Microsoft.Extensions.Logging;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    public class AdminPartsService : IAdminPartsService
    {
        private readonly IAdminPartsRepository _adminPartsRepository;
        private readonly ILogger<AdminPartsService> _logger;

        public AdminPartsService(
            IAdminPartsRepository adminPartsRepository,
            ILogger<AdminPartsService> logger)
        {
            _adminPartsRepository = adminPartsRepository;
            _logger = logger;
        }

        // Returns all parts for admin view.
        public async Task<List<VehiclePart>> GetAllPartsAsync()
        {
            return await _adminPartsRepository.GetAllPartsAsync();
        }

        // Handles purchase process including stock update and invoice creation.
        public async Task<object> PurchasePartsAsync(CreatePurchaseDto dto)
        {
            // Checks if items list is empty.
            if (dto.Items == null || !dto.Items.Any())
                return new { success = false, message = "At least one purchase item is required." };

            // Validates payment status.
            if (string.IsNullOrWhiteSpace(dto.Payment_Status))
                return new { success = false, message = "Payment status is required." };

            // Checks vendor exists.
            var vendorExists = await _adminPartsRepository.VendorExistsAsync(dto.Vendor_ID);
            if (!vendorExists)
                return new { success = false, message = "Vendor not found." };

            // Checks admin exists.
            var adminExists = await _adminPartsRepository.AdminExistsAsync(dto.Admin_ID);
            if (!adminExists)
                return new { success = false, message = "Admin not found." };

            // Starts transaction because multiple operations are involved.
            await _adminPartsRepository.BeginTransactionAsync();

            try
            {
                decimal totalCost = 0;
                var purchaseItems = new List<PurchaseItem>();

                foreach (var item in dto.Items)
                {
                    // Gets part from database.
                    var part = await _adminPartsRepository.GetPartByIdAsync(item.Part_ID);

                    if (part == null)
                        return new { success = false, message = $"Part ID {item.Part_ID} not found." };

                    // Calculates cost of each item.
                    var lineTotal = item.Quantity_Purchased * item.Purchase_Unit_Cost;
                    totalCost += lineTotal;

                    // Updates stock after purchase.
                    part.Stock_Quantity += item.Quantity_Purchased;
                    part.Purchase_Price = item.Purchase_Unit_Cost;
                    part.IsAvailable = part.Stock_Quantity > 0;
                    part.Updated_At = DateTime.UtcNow;

                    purchaseItems.Add(new PurchaseItem
                    {
                        Part_ID = item.Part_ID,
                        Quantity_Purchased = item.Quantity_Purchased,
                        Purchase_Unit_Cost = item.Purchase_Unit_Cost,
                        Line_Total = lineTotal
                    });
                }

                // Creates purchase invoice.
                var invoice = new PurchaseInvoice
                {
                    Vendor_ID = dto.Vendor_ID,
                    Admin_ID = dto.Admin_ID,
                    Purchase_Date = DateTime.UtcNow,
                    Total_Cost = totalCost,
                    Payment_Status = dto.Payment_Status.Trim(),
                    Created_At = DateTime.UtcNow
                };

                await _adminPartsRepository.AddPurchaseInvoiceAsync(invoice);
                await _adminPartsRepository.SaveChangesAsync();

                // Links items to invoice.
                foreach (var purchaseItem in purchaseItems)
                {
                    purchaseItem.Purchase_Invoice_No = invoice.Purchase_Invoice_No;
                    await _adminPartsRepository.AddPurchaseItemAsync(purchaseItem);
                }

                await _adminPartsRepository.SaveChangesAsync();

                // Saves all changes.
                await _adminPartsRepository.CommitTransactionAsync();

                _logger.LogInformation("Purchase invoice created successfully.");

                return new
                {
                    success = true,
                    message = "Parts purchased successfully.",
                    invoiceNo = invoice.Purchase_Invoice_No,
                    totalCost = invoice.Total_Cost
                };
            }
            catch (Exception ex)
            {
                // Rolls back changes if error occurs.
                await _adminPartsRepository.RollbackTransactionAsync();

                _logger.LogError(ex, "Error while purchasing parts.");

                return new
                {
                    success = false,
                    message = "An error occurred while purchasing parts."
                };
            }
        }

        // Updates part details like name, price, and stock.
        public async Task<string> UpdatePartAsync(int partId, UpdateVehiclePartDto dto)
        {
            var part = await _adminPartsRepository.GetPartByIdAsync(partId);

            if (part == null)
                return "Part not found.";

            // Updates all editable fields.
            part.Part_Name = dto.Part_Name.Trim();
            part.Part_Category = dto.Part_Category.Trim();
            part.Brand = dto.Brand.Trim();
            part.Stock_Quantity = dto.Stock_Quantity;
            part.Minimum_Stock_Level = dto.Minimum_Stock_Level;
            part.Unit_Price = dto.Unit_Price;
            part.Purchase_Price = dto.Purchase_Price;
            part.IsAvailable = dto.Stock_Quantity > 0;
            part.Updated_At = DateTime.UtcNow;

            await _adminPartsRepository.SaveChangesAsync();

            return "Part updated successfully.";
        }

        // Soft deletes part by marking it unavailable.
        public async Task<string> DeletePartAsync(int partId)
        {
            var part = await _adminPartsRepository.GetPartByIdAsync(partId);

            if (part == null)
                return "Part not found.";

            part.IsAvailable = false;
            part.Stock_Quantity = 0;
            part.Updated_At = DateTime.UtcNow;

            await _adminPartsRepository.SaveChangesAsync();

            return "Part deleted successfully.";
        }
    }
}