using Microsoft.EntityFrameworkCore;
using VehicleInventoryManagementSystem.Application.DTOs.Reports;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Handles database queries for staff reports.
    /// </summary>
    public class StaffReportRepository : IStaffReportRepository
    {
        private readonly AppDbContext _context;

        public StaffReportRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns customers who purchase frequently.
        /// </summary>
        public async Task<List<RegularCustomerReportDto>> GetRegularCustomersAsync(int minimumPurchases, int limit)
        {
            return await _context.SalesInvoices
                .AsNoTracking()
                .Where(invoice=> invoice.Customer != null && invoice.Customer.User != null) // null safety ✔
                .GroupBy(invoice => new
                {
                    invoice.Customer_ID,
                    invoice.Customer.User.FullName,
                    invoice.Customer.User.PhoneNumber
                })
                .Select(group => new RegularCustomerReportDto
                {
                    Customer_ID = group.Key.Customer_ID,
                    FullName = group.Key.FullName,
                    PhoneNumber = group.Key.PhoneNumber,
                    TotalPurchases = group.Count(),
                    TotalSpent = group.Sum(invoice => invoice.Final_Total),
                    LastPurchaseDate = group.Max(invoice => invoice.Sales_Date)
                })
                .Where(customer => customer.TotalPurchases >= minimumPurchases)
                .OrderByDescending(customer => customer.TotalPurchases)
                .ThenByDescending(customer => customer.TotalSpent)
                .Take(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Returns top spending customers.
        /// </summary>
        public async Task<List<HighSpenderReportDto>> GetHighSpendersAsync(int limit)
        {
            return await _context.SalesInvoices
                .AsNoTracking()
                .Where(invoice => invoice.Customer != null && invoice.Customer.User != null)
                .GroupBy(invoice => new
                {
                    invoice.Customer_ID,
                    invoice.Customer.User.FullName,
                    invoice.Customer.User.PhoneNumber
                })
                .Select(group => new HighSpenderReportDto
                {
                    Customer_ID = group.Key.Customer_ID,
                    FullName = group.Key.FullName,
                    PhoneNumber = group.Key.PhoneNumber,
                    TotalSpent = group.Sum(invoice => invoice.Final_Total),
                    TotalInvoices = group.Count()
                })
                .OrderByDescending(customer => customer.TotalSpent)
                .Take(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Returns unpaid invoices.
        /// </summary>
        public async Task<List<PendingCreditReportDto>> GetPendingCreditsAsync()
        {
            return await _context.SalesInvoices
                .AsNoTracking()
                .Where(invoice => !invoice.Is_Paid && invoice.Customer != null && invoice.Customer.User != null)
                .OrderBy(invoice => invoice.Credit_Due_Date)
                .Select(invoice => new PendingCreditReportDto
                {
                    Customer_ID = invoice.Customer_ID,
                    FullName = invoice.Customer.User.FullName,
                    PhoneNumber = invoice.Customer.User.PhoneNumber,
                    Sales_Invoice_No = invoice.Sales_Invoice_No,
                    PendingAmount = invoice.Final_Total,
                    Credit_Due_Date = invoice.Credit_Due_Date,
                    IsOverdue = invoice.Credit_Due_Date.HasValue &&
                                invoice.Credit_Due_Date.Value.Date < DateTime.UtcNow.Date
                })
                .ToListAsync();
        }
    }
}