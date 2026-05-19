using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminPurchaseInvoiceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminPurchaseInvoiceController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Create a new purchase invoice and update stock
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateInvoice([FromBody] CreatePurchaseInvoiceRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if vendor exists
            var vendor = await _context.Vendors.FindAsync(request.VendorId);
            if (vendor == null)
                return NotFound(new { message = "Vendor not found" });

            // Check if admin exists
            var admin = await _context.Admins.FindAsync(request.AdminId);
            if (admin == null)
                return NotFound(new { message = "Admin not found" });

            // Generate unique invoice number
            var invoiceNumber = GenerateInvoiceNumber();

            // Create invoice
            var invoice = new PurchaseInvoice
            {
                Vendor_ID = request.VendorId,
                Admin_ID = request.AdminId,
                Invoice_Number = invoiceNumber,
                Purchase_Date = DateTime.UtcNow,
                Payment_Status = "Pending",
                Notes = request.Notes,
                Created_At = DateTime.UtcNow,
                PurchaseItems = new List<PurchaseItem>()
            };

            decimal subTotal = 0;

            // Process each item
            foreach (var itemDto in request.Items)
            {
                // Check if part exists
                var part = await _context.VehicleParts.FindAsync(itemDto.PartId);
                if (part == null)
                    return NotFound(new { message = $"Part with ID {itemDto.PartId} not found" });

                var itemTotal = itemDto.Quantity * itemDto.UnitPrice;
                subTotal += itemTotal;

                // Add invoice item using correct property names
                invoice.PurchaseItems.Add(new PurchaseItem
                {
                    Part_ID = itemDto.PartId,
                    Quantity_Purchased = itemDto.Quantity,
                    Purchase_Unit_Cost = itemDto.UnitPrice,
                    Line_Total = itemTotal
                });

                // Update stock quantity
                part.Stock_Quantity += itemDto.Quantity;
                part.Updated_At = DateTime.UtcNow;
            }

            // Calculate totals (13% VAT)
            var taxAmount = subTotal * 0.13m;
            var totalAmount = subTotal + taxAmount;

            invoice.Sub_Total = subTotal;
            invoice.Tax_Amount = taxAmount;
            invoice.Total_Cost = totalAmount;

            _context.PurchaseInvoices.Add(invoice);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Purchase invoice created successfully",
                invoiceId = invoice.Purchase_Invoice_No,
                invoiceNumber = invoice.Invoice_Number,
                subTotal = invoice.Sub_Total,
                taxAmount = invoice.Tax_Amount,
                totalCost = invoice.Total_Cost,
                itemsCount = invoice.PurchaseItems.Count
            });
        }

        /// <summary>
        /// Get all purchase invoices
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices = await _context.PurchaseInvoices
                .Include(i => i.Vendor)
                .Include(i => i.PurchaseItems)
                .ThenInclude(i => i.VehiclePart)
                .OrderByDescending(i => i.Purchase_Date)
                .Select(i => new
                {
                    i.Purchase_Invoice_No,
                    i.Invoice_Number,
                    i.Purchase_Date,
                    VendorName = i.Vendor != null ? i.Vendor.Vendor_Name : "Unknown",
                    i.Sub_Total,
                    i.Tax_Amount,
                    i.Total_Cost,
                    i.Payment_Status,
                    ItemCount = i.PurchaseItems.Count
                })
                .ToListAsync();

            return Ok(invoices);
        }

        /// <summary>
        /// Get purchase invoice by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var invoice = await _context.PurchaseInvoices
                .Include(i => i.Vendor)
                .Include(i => i.PurchaseItems)
                .ThenInclude(i => i.VehiclePart)
                .FirstOrDefaultAsync(i => i.Purchase_Invoice_No == id);

            if (invoice == null)
                return NotFound(new { message = "Invoice not found" });

            return Ok(new
            {
                invoice.Purchase_Invoice_No,
                invoice.Invoice_Number,
                invoice.Purchase_Date,
                VendorName = invoice.Vendor?.Vendor_Name,
                invoice.Sub_Total,
                invoice.Tax_Amount,
                invoice.Total_Cost,
                invoice.Payment_Status,
                invoice.Notes,
                Items = invoice.PurchaseItems.Select(i => new
                {
                    PartName = i.VehiclePart?.Part_Name,
                    i.Quantity_Purchased,
                    i.Purchase_Unit_Cost,
                    i.Line_Total
                })
            });
        }

        /// <summary>
        /// Update payment status of an invoice
        /// </summary>
        [HttpPut("{id}/payment-status")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] string paymentStatus)
        {
            var invoice = await _context.PurchaseInvoices.FindAsync(id);
            if (invoice == null)
                return NotFound(new { message = "Invoice not found" });

            invoice.Payment_Status = paymentStatus;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Payment status updated successfully" });
        }

        private string GenerateInvoiceNumber()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var count = _context.PurchaseInvoices
                .Count(i => i.Invoice_Number != null && i.Invoice_Number.StartsWith($"PO-{datePart}")) + 1;
            return $"PO-{datePart}-{count:D4}";
        }
    }
}