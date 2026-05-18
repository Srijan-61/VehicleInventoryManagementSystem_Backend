using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VehicleInventoryManagementSystem.Application.DTOs.PurchaseInvoice;
using VehicleInventoryManagementSystem.Application.Interfaces.PurchaseInvoice;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;
using DomainPurchaseInvoice = VehicleInventoryManagementSystem.Domain.Models.PurchaseInvoice;

namespace VehicleInventoryManagementSystem.Infrastructure.Services.PurchaseInvoice
{
    /// <summary>
    /// Creates purchase invoices and updates VehiclePart stock atomically.
    /// All writes happen inside a database transaction so a partial failure
    /// cannot corrupt stock counts.
    /// </summary>
    public class PurchaseInvoiceService : IPurchaseInvoiceService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PurchaseInvoiceService> _logger;

        public PurchaseInvoiceService(
            AppDbContext dbContext,
            ILogger<PurchaseInvoiceService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<PurchaseInvoiceResultDto> CreateAsync(
            CreatePurchaseInvoiceRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate references exist.
            var vendor = await _dbContext.Vendors
                .FirstOrDefaultAsync(v => v.Vendor_ID == request.VendorId, cancellationToken);
            if (vendor is null)
            {
                return PurchaseInvoiceResultDto.Failure(
                    new[] { $"Vendor with ID {request.VendorId} not found." });
            }

            var admin = await _dbContext.Admins
                .FirstOrDefaultAsync(a => a.Admin_ID == request.AdminId, cancellationToken);
            if (admin is null)
            {
                return PurchaseInvoiceResultDto.Failure(
                    new[] { $"Admin with ID {request.AdminId} not found." });
            }

            // 2. Reject duplicate part IDs in the same invoice.
            //    The composite primary key (Invoice_No, Part_ID) on PurchaseItem
            //    forbids them anyway, but a clear error is friendlier.
            var duplicatePartIds = request.Items
                .GroupBy(i => i.PartId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicatePartIds.Any())
            {
                return PurchaseInvoiceResultDto.Failure(new[]
                {
                    $"Duplicate part IDs in invoice: {string.Join(", ", duplicatePartIds)}. " +
                    "Combine them into a single line item."
                });
            }

            // 3. Load all referenced parts in one query (avoids N+1).
            var partIds = request.Items.Select(i => i.PartId).ToList();
            var parts = await _dbContext.VehicleParts
                .Where(p => partIds.Contains(p.Part_ID))
                .ToDictionaryAsync(p => p.Part_ID, cancellationToken);

            var missingPartIds = partIds.Where(id => !parts.ContainsKey(id)).ToList();
            if (missingPartIds.Any())
            {
                return PurchaseInvoiceResultDto.Failure(new[]
                {
                    $"Part IDs not found: {string.Join(", ", missingPartIds)}."
                });
            }

            // 4. Build the invoice + items + apply stock changes inside a transaction.
            await using var transaction = await _dbContext.Database
                .BeginTransactionAsync(cancellationToken);

            try
            {
                var now = DateTime.UtcNow;

                var invoice = new DomainPurchaseInvoice
                {
                    Vendor_ID = request.VendorId,
                    Admin_ID = request.AdminId,
                    Purchase_Date = DateTime.SpecifyKind(request.PurchaseDate, DateTimeKind.Utc),
                    Payment_Status = request.PaymentStatus,
                    Total_Cost = 0m, // computed below
                    Created_At = now
                };

                _dbContext.PurchaseInvoices.Add(invoice);
                // Save once to get the auto-generated Purchase_Invoice_No.
                await _dbContext.SaveChangesAsync(cancellationToken);

                decimal runningTotal = 0m;
                var itemEntities = new List<PurchaseItem>();

                foreach (var line in request.Items)
                {
                    var part = parts[line.PartId];
                    var lineTotal = line.QuantityPurchased * line.PurchaseUnitCost;
                    runningTotal += lineTotal;

                    itemEntities.Add(new PurchaseItem
                    {
                        Purchase_Invoice_No = invoice.Purchase_Invoice_No,
                        Part_ID = line.PartId,
                        Quantity_Purchased = line.QuantityPurchased,
                        Purchase_Unit_Cost = line.PurchaseUnitCost,
                        Line_Total = lineTotal
                    });

                    // The core of F4 — increment stock for each part purchased.
                    part.Stock_Quantity += line.QuantityPurchased;
                    part.Updated_At = now;
                    // A previously-out-of-stock part is now available.
                    if (part.Stock_Quantity > 0)
                    {
                        part.IsAvailable = true;
                    }
                }

                _dbContext.PurchaseItems.AddRange(itemEntities);
                invoice.Total_Cost = runningTotal;

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation(
                    "Purchase invoice {InvoiceNo} created with {ItemCount} item(s), total {Total}.",
                    invoice.Purchase_Invoice_No, itemEntities.Count, runningTotal);

                return PurchaseInvoiceResultDto.Success(new PurchaseInvoiceResponse
                {
                    PurchaseInvoiceNo = invoice.Purchase_Invoice_No,
                    VendorId = vendor.Vendor_ID,
                    VendorName = vendor.Vendor_Name,
                    AdminId = admin.Admin_ID,
                    PurchaseDate = invoice.Purchase_Date,
                    TotalCost = invoice.Total_Cost,
                    PaymentStatus = invoice.Payment_Status,
                    CreatedAt = invoice.Created_At,
                    Items = itemEntities.Select(i => new PurchaseItemResponse
                    {
                        PartId = i.Part_ID,
                        PartName = parts[i.Part_ID].Part_Name,
                        QuantityPurchased = i.Quantity_Purchased,
                        PurchaseUnitCost = i.Purchase_Unit_Cost,
                        LineTotal = i.Line_Total
                    }).ToList()
                });
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Database error while creating purchase invoice.");
                return PurchaseInvoiceResultDto.Failure(
                    new[] { "Could not save purchase invoice due to a database error." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Unexpected error while creating purchase invoice.");
                return PurchaseInvoiceResultDto.Failure(
                    new[] { "An unexpected error occurred. Please try again." });
            }
        }

        public async Task<List<PurchaseInvoiceSummaryResponse>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.PurchaseInvoices
                .AsNoTracking()
                .Include(pi => pi.Vendor)
                .OrderByDescending(pi => pi.Purchase_Date)
                .Select(pi => new PurchaseInvoiceSummaryResponse
                {
                    PurchaseInvoiceNo = pi.Purchase_Invoice_No,
                    VendorName = pi.Vendor.Vendor_Name,
                    PurchaseDate = pi.Purchase_Date,
                    TotalCost = pi.Total_Cost,
                    PaymentStatus = pi.Payment_Status,
                    ItemCount = _dbContext.PurchaseItems
                        .Count(it => it.Purchase_Invoice_No == pi.Purchase_Invoice_No)
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<PurchaseInvoiceResponse?> GetByIdAsync(
            int purchaseInvoiceNo,
            CancellationToken cancellationToken = default)
        {
            var invoice = await _dbContext.PurchaseInvoices
                .AsNoTracking()
                .Include(pi => pi.Vendor)
                .FirstOrDefaultAsync(pi => pi.Purchase_Invoice_No == purchaseInvoiceNo,
                                     cancellationToken);

            if (invoice is null)
            {
                return null;
            }

            var items = await _dbContext.PurchaseItems
                .AsNoTracking()
                .Where(it => it.Purchase_Invoice_No == purchaseInvoiceNo)
                .Include(it => it.VehiclePart)
                .Select(it => new PurchaseItemResponse
                {
                    PartId = it.Part_ID,
                    PartName = it.VehiclePart.Part_Name,
                    QuantityPurchased = it.Quantity_Purchased,
                    PurchaseUnitCost = it.Purchase_Unit_Cost,
                    LineTotal = it.Line_Total
                })
                .ToListAsync(cancellationToken);

            return new PurchaseInvoiceResponse
            {
                PurchaseInvoiceNo = invoice.Purchase_Invoice_No,
                VendorId = invoice.Vendor_ID,
                VendorName = invoice.Vendor.Vendor_Name,
                AdminId = invoice.Admin_ID,
                PurchaseDate = invoice.Purchase_Date,
                TotalCost = invoice.Total_Cost,
                PaymentStatus = invoice.Payment_Status,
                CreatedAt = invoice.Created_At,
                Items = items
            };
        }
    }
}