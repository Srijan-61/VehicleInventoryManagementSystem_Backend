using Microsoft.EntityFrameworkCore;
using VehicleInventoryManagementSystem.Application.DTOs.Invoices;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Reads invoice details and invoice items for staff invoice email feature.
    /// </summary>
    public class InvoiceEmailRepository : IInvoiceEmailRepository
    {
        private readonly AppDbContext _context;

        public InvoiceEmailRepository(AppDbContext context)
        {
            _context = context;
        }

        // Gets invoices of selected customer that have at least one line item.
        // Invoices with no items are excluded from the dropdown — they cannot be
        // previewed or emailed meaningfully and are likely data anomalies.
        public async Task<List<CustomerInvoiceDropdownDto>> GetInvoicesByCustomerAsync(int customerId)
        {
            return await _context.SalesInvoices
                .AsNoTracking()
                .Where(invoice =>
                    invoice.Customer_ID == customerId &&
                    _context.SalesItems.Any(si => si.Sales_Invoice_No == invoice.Sales_Invoice_No))
                .OrderByDescending(invoice => invoice.Sales_Date)
                .Select(invoice => new CustomerInvoiceDropdownDto
                {
                    Sales_Invoice_No = invoice.Sales_Invoice_No,
                    Sales_Date = invoice.Sales_Date,
                    Final_Total = invoice.Final_Total,
                    Is_Paid = invoice.Is_Paid
                })
                .ToListAsync();
        }

        // Gets invoice header and items for invoice preview and email body.
        public async Task<InvoiceEmailDetailsDto?> GetInvoiceEmailDetailsAsync(
            int customerId,
            int salesInvoiceNo)
        {
            var invoice = await _context.SalesInvoices
                .AsNoTracking()
                .Where(invoice =>
                    invoice.Customer_ID == customerId &&
                    invoice.Sales_Invoice_No == salesInvoiceNo)
                .Select(invoice => new InvoiceEmailDetailsDto
                {
                    Sales_Invoice_No = invoice.Sales_Invoice_No,
                    Customer_ID = invoice.Customer_ID,
                    CustomerName = invoice.Customer.User.FullName ?? "Unknown Customer",
                    CustomerEmail = invoice.Customer.User.Email ?? string.Empty,
                    CustomerPhone = invoice.Customer.User.PhoneNumber,
                    StaffName = invoice.Staff.User.FullName ?? "Staff",
                    Sales_Date = invoice.Sales_Date,
                    Sub_Total = invoice.Sub_Total,
                    Discount_Amount = invoice.Discount_Amount,
                    Final_Total = invoice.Final_Total,
                    Is_Paid = invoice.Is_Paid,
                    Credit_Due_Date = invoice.Credit_Due_Date
                })
                .FirstOrDefaultAsync();

            if (invoice == null)
                return null;

            // Load sold items for this invoice.
            // This explicit join avoids empty navigation-property issues.
            invoice.Items = await (
                from salesItem in _context.SalesItems.AsNoTracking()
                join part in _context.VehicleParts.AsNoTracking()
                    on salesItem.Part_ID equals part.Part_ID
                where salesItem.Sales_Invoice_No == salesInvoiceNo
                orderby part.Part_Name
                select new InvoiceEmailItemDto
                {
                    PartName = part.Part_Name,
                    Brand = part.Brand,
                    Quantity_Sold = salesItem.Quantity_Sold,
                    Unit_Price = salesItem.Unit_Price,
                    Total_Price = salesItem.Total_Price
                }
            ).ToListAsync();

            return invoice;
        }
    }
}