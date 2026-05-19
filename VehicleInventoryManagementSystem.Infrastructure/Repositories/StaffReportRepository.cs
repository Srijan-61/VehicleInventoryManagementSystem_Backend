using Microsoft.EntityFrameworkCore;
using VehicleInventoryManagementSystem.Application.DTOs.Reports;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Handles database queries for staff customer reports.
    /// </summary>
    public class StaffReportRepository : IStaffReportRepository
    {
        private readonly AppDbContext _context;

        public StaffReportRepository(AppDbContext context)
        {
            _context = context;
        }

        // Returns customers who purchase frequently.
        public async Task<List<RegularCustomerReportDto>> GetRegularCustomersAsync(
            int minimumPurchases,
            int limit,
            DateTime? startDate,
            DateTime? endDate)
        {
            var query = ApplyDateFilter(
                _context.SalesInvoices.AsNoTracking(),
                startDate,
                endDate
            );

            return await query
                .Where(invoice =>
                    invoice.Customer != null &&
                    invoice.Customer.User != null)
                .GroupBy(invoice => new
                {
                    invoice.Customer_ID,
                    invoice.Customer.User.FullName,
                    invoice.Customer.User.Email,
                    invoice.Customer.User.PhoneNumber
                })
                .Select(group => new RegularCustomerReportDto
                {
                    Customer_ID = group.Key.Customer_ID,
                    FullName = group.Key.FullName,
                    Email = group.Key.Email,
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

        // Returns top spending customers.
        public async Task<List<HighSpenderReportDto>> GetHighSpendersAsync(
            int limit,
            decimal? minimumSpent,
            DateTime? startDate,
            DateTime? endDate)
        {
            var query = ApplyDateFilter(
                _context.SalesInvoices.AsNoTracking(),
                startDate,
                endDate
            );

            var reportQuery = query
                .Where(invoice =>
                    invoice.Customer != null &&
                    invoice.Customer.User != null)
                .GroupBy(invoice => new
                {
                    invoice.Customer_ID,
                    invoice.Customer.User.FullName,
                    invoice.Customer.User.Email,
                    invoice.Customer.User.PhoneNumber
                })
                .Select(group => new HighSpenderReportDto
                {
                    Customer_ID = group.Key.Customer_ID,
                    FullName = group.Key.FullName,
                    Email = group.Key.Email,
                    PhoneNumber = group.Key.PhoneNumber,
                    TotalSpent = group.Sum(invoice => invoice.Final_Total),
                    TotalInvoices = group.Count(),
                    LastPurchaseDate = group.Max(invoice => invoice.Sales_Date)
                });

            if (minimumSpent.HasValue && minimumSpent.Value > 0)
            {
                reportQuery = reportQuery
                    .Where(customer => customer.TotalSpent >= minimumSpent.Value);
            }

            return await reportQuery
                .OrderByDescending(customer => customer.TotalSpent)
                .Take(limit)
                .ToListAsync();
        }

        // Returns unpaid invoices. Can optionally show overdue records only.
        public async Task<List<PendingCreditReportDto>> GetPendingCreditsAsync(
            bool overdueOnly)
        {
            var today = DateTime.UtcNow.Date;

            var query = _context.SalesInvoices
                .AsNoTracking()
                .Where(invoice =>
                    !invoice.Is_Paid &&
                    invoice.Customer != null &&
                    invoice.Customer.User != null);

            if (overdueOnly)
            {
                query = query.Where(invoice =>
                    invoice.Credit_Due_Date.HasValue &&
                    invoice.Credit_Due_Date.Value.Date < today);
            }

            return await query
                .OrderBy(invoice => invoice.Credit_Due_Date)
                .Select(invoice => new PendingCreditReportDto
                {
                    Customer_ID = invoice.Customer_ID,
                    FullName = invoice.Customer.User.FullName,
                    Email = invoice.Customer.User.Email,
                    PhoneNumber = invoice.Customer.User.PhoneNumber,
                    Sales_Invoice_No = invoice.Sales_Invoice_No,
                    PendingAmount = invoice.Final_Total,
                    Credit_Due_Date = invoice.Credit_Due_Date,
                    IsOverdue = invoice.Credit_Due_Date.HasValue &&
                                invoice.Credit_Due_Date.Value.Date < today
                })
                .ToListAsync();
        }

        // Applies optional sales date filter to report queries.
        private static IQueryable<SalesInvoice> ApplyDateFilter(
            IQueryable<SalesInvoice> query,
            DateTime? startDate,
            DateTime? endDate)
        {
            if (startDate.HasValue)
            {
                query = query.Where(invoice =>
                    invoice.Sales_Date.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(invoice =>
                    invoice.Sales_Date.Date <= endDate.Value.Date);
            }

            return query;
        }
    }
}