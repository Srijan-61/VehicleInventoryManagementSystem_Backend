using Microsoft.EntityFrameworkCore;
using VehicleInventoryManagementSystem.Application.DTOs.Invoices;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Reads invoice, customer, staff, and item details from the database for invoice email.
    /// </summary>
    public class InvoiceEmailRepository : IInvoiceEmailRepository
    {
        private readonly AppDbContext _context;

        public InvoiceEmailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceEmailDetailsDto?> GetInvoiceEmailDetailsAsync(int salesInvoiceNo)
        {
            var invoice = await _context.SalesInvoices
                .AsNoTracking()
                .Where(invoice => invoice.Sales_Invoice_No == salesInvoiceNo)
                .Select(invoice => new InvoiceEmailDetailsDto
                {
                    Sales_Invoice_No = invoice.Sales_Invoice_No,
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

            invoice.Items = await _context.SalesItems
                .AsNoTracking()
                .Where(item => item.Sales_Invoice_No == salesInvoiceNo)
                .Select(item => new InvoiceEmailItemDto
                {
                    PartName = item.VehiclePart.Part_Name,
                    Brand = item.VehiclePart.Brand,
                    Quantity_Sold = item.Quantity_Sold,
                    Unit_Price = item.Unit_Price,
                    Total_Price = item.Total_Price
                })
                .ToListAsync();

            return invoice;
        }
    }
}