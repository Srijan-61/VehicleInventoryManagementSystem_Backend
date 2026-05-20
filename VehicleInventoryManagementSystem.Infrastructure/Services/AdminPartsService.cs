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
        public async Task<object> PurchasePartsAsync(CreatePurchaseDto dto, string userId)
        {
            // Checks if logged-in admin user id exists.
            if (string.IsNullOrWhiteSpace(userId))
                return new { success = false, message = "Invalid admin token." };

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

            // Gets admin id from logged-in JWT user id.
            var adminId = await _adminPartsRepository.GetAdminIdByUserIdAsync(userId);

            if (adminId == null)
                return new { success = false, message = "Admin profile not found." };

            // Starts transaction because stock, invoice, and items must save together.
            await _adminPartsRepository.BeginTransactionAsync();

            try
            {
                decimal totalCost = 0;
                var purchaseItems = new List<PurchaseItem>();

                foreach (var item in dto.Items)
                {
                    // Gets part from database.
                    var part = await _adminPartsRepository.GetPartByIdAsync(item.Part_ID);
                    // Rollback is needed because transaction has already started.
                    if (part == null)
                    {
                        await _adminPartsRepository.RollbackTransactionAsync();

                        return new
                        {
                            success = false,
                            message = $"Part ID {item.Part_ID} not found."
                        };
                    }
                    // Extra validation for safety.
                    if (item.Quantity_Purchased <= 0)
                    {
                        await _adminPartsRepository.RollbackTransactionAsync();

                        return new
                        {
                            success = false,
                            message = "Quantity purchased must be greater than zero."
                        };
                    }

                    if (item.Purchase_Unit_Cost <= 0)
                    {
                        await _adminPartsRepository.RollbackTransactionAsync();

                        return new
                        {
                            success = false,
                            message = "Purchase unit cost must be greater than zero."
                        };
                    }

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
                    Admin_ID = adminId.Value,
                    Purchase_Date = DateTime.UtcNow,
                    Total_Cost = totalCost,
                    Payment_Status = dto.Payment_Status.Trim(),
                    Created_At = DateTime.UtcNow
                };

                await _adminPartsRepository.AddPurchaseInvoiceAsync(invoice);
                await _adminPartsRepository.SaveChangesAsync();

                // Links all purchase items to generated purchase invoice number.
                foreach (var purchaseItem in purchaseItems)
                {
                    purchaseItem.Purchase_Invoice_No = invoice.Purchase_Invoice_No;
                    await _adminPartsRepository.AddPurchaseItemAsync(purchaseItem);
                }

                await _adminPartsRepository.SaveChangesAsync();

                // Saves all changes.
                await _adminPartsRepository.CommitTransactionAsync();

                _logger.LogInformation(
                    "Purchase invoice {InvoiceNo} created successfully by admin {AdminId}.",
                    invoice.Purchase_Invoice_No,
                    adminId.Value
                );

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
                // Rolls back stock, invoice, and item changes if any error occurs.
                await _adminPartsRepository.RollbackTransactionAsync();

                _logger.LogError(ex, "Error while purchasing parts.");

                return new
                {
                    success = false,
                    message = "An error occurred while purchasing parts."
                };
            }
        }

        // Creates a brand-new inventory part record and immediately records its first purchase.
        // Runs inside a transaction so both the part and invoice are saved together or not at all.
        public async Task<object> CreateNewPartAndPurchaseAsync(CreateNewPartPurchaseDto dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new { success = false, message = "Invalid admin token." };

            var vendorExists = await _adminPartsRepository.VendorExistsAsync(dto.Vendor_ID);
            if (!vendorExists)
                return new { success = false, message = "Vendor not found." };

            // Duplicate guard — prevents two entries with the same name + brand.
            var duplicate = await _adminPartsRepository.PartExistsAsync(dto.Part_Name.Trim(), dto.Brand.Trim());
            if (duplicate)
                return new
                {
                    success = false,
                    message = $"A part named \"{dto.Part_Name}\" by \"{dto.Brand}\" already exists. Use \"Existing Part\" purchase instead."
                };

            var adminId = await _adminPartsRepository.GetAdminIdByUserIdAsync(userId);
            if (adminId == null)
                return new { success = false, message = "Admin profile not found." };

            await _adminPartsRepository.BeginTransactionAsync();
            try
            {
                // 1. Create the new VehiclePart row.
                var newPart = new VehiclePart
                {
                    Part_Name          = dto.Part_Name.Trim(),
                    Brand              = dto.Brand.Trim(),
                    Part_Category      = dto.Part_Category.Trim(),
                    Unit_Price         = dto.Selling_Price,
                    Purchase_Price     = dto.Purchase_Unit_Cost,
                    Stock_Quantity     = dto.Quantity_Purchased,
                    Minimum_Stock_Level = 5,
                    IsAvailable        = true,
                    Created_At         = DateTime.UtcNow,
                    Updated_At         = DateTime.UtcNow,
                };
                await _adminPartsRepository.AddPartAsync(newPart);
                await _adminPartsRepository.SaveChangesAsync(); // flush to get the generated Part_ID

                // 2. Create the purchase invoice.
                var lineTotal = dto.Quantity_Purchased * dto.Purchase_Unit_Cost;
                var invoice = new PurchaseInvoice
                {
                    Vendor_ID      = dto.Vendor_ID,
                    Admin_ID       = adminId.Value,
                    Purchase_Date  = DateTime.UtcNow,
                    Total_Cost     = lineTotal,
                    Payment_Status = dto.Payment_Status.Trim(),
                    Created_At     = DateTime.UtcNow,
                };
                await _adminPartsRepository.AddPurchaseInvoiceAsync(invoice);
                await _adminPartsRepository.SaveChangesAsync(); // flush to get Purchase_Invoice_No

                // 3. Link the purchase item to the invoice and the new part.
                var purchaseItem = new PurchaseItem
                {
                    Purchase_Invoice_No = invoice.Purchase_Invoice_No,
                    Part_ID             = newPart.Part_ID,
                    Quantity_Purchased  = dto.Quantity_Purchased,
                    Purchase_Unit_Cost  = dto.Purchase_Unit_Cost,
                    Line_Total          = lineTotal,
                };
                await _adminPartsRepository.AddPurchaseItemAsync(purchaseItem);
                await _adminPartsRepository.SaveChangesAsync();

                await _adminPartsRepository.CommitTransactionAsync();

                _logger.LogInformation(
                    "New part '{PartName}' (ID {PartId}) created and purchase invoice {InvoiceNo} recorded by admin {AdminId}.",
                    newPart.Part_Name, newPart.Part_ID, invoice.Purchase_Invoice_No, adminId.Value);

                return new
                {
                    success   = true,
                    message   = $"New part \"{newPart.Part_Name}\" added to inventory and purchase recorded successfully.",
                    partId    = newPart.Part_ID,
                    invoiceNo = invoice.Purchase_Invoice_No,
                    totalCost = invoice.Total_Cost,
                };
            }
            catch (Exception ex)
            {
                await _adminPartsRepository.RollbackTransactionAsync();
                _logger.LogError(ex, "Error while creating new part and purchase.");
                return new { success = false, message = "An error occurred while saving the new part." };
            }
        }

        // Updates editable part details — stock quantity is intentionally excluded
        // so that stock can only change via purchase invoices, keeping records consistent.
        public async Task<string> UpdatePartAsync(int partId, UpdateVehiclePartDto dto)
        {
            var part = await _adminPartsRepository.GetPartByIdAsync(partId);

            if (part == null)
                return "Part not found.";

            part.Part_Name           = dto.Part_Name.Trim();
            part.Part_Category       = dto.Part_Category.Trim();
            part.Brand               = dto.Brand.Trim();
            part.Minimum_Stock_Level = dto.Minimum_Stock_Level;
            part.Unit_Price          = dto.Unit_Price;
            part.IsAvailable         = dto.IsAvailable;
            part.Updated_At          = DateTime.UtcNow;

            await _adminPartsRepository.SaveChangesAsync();

            return "Part updated successfully.";
        }

        // Soft deletes part by marking it unavailable.
        public async Task<string> DeletePartAsync(int partId)
        {
            var part = await _adminPartsRepository.GetPartByIdAsync(partId);

            if (part == null)
                return "Part not found.";

            // Soft delete keeps old sales and purchase history safe.
            part.IsAvailable = false;
            part.Stock_Quantity = 0;
            part.Updated_At = DateTime.UtcNow;

            await _adminPartsRepository.SaveChangesAsync();

            return "Part deleted successfully.";
        }
    }
}