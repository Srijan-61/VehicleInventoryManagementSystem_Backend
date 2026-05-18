using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class FinancialReportController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FinancialReportController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get daily financial report
        /// </summary>
        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyReport(string date)
        {
            // Validate and parse the date
            if (!DateTime.TryParse(date, out DateTime parsedDate))
            {
                return BadRequest(new { message = "Invalid date format. Please use YYYY-MM-DD" });
            }

            var startDate = parsedDate.Date.ToUniversalTime();
            var endDate = startDate.AddDays(1);

            // Total sales for the day (using Sales_Date and Final_Total)
            var totalSales = await _context.SalesInvoices
                .Where(s => s.Sales_Date >= startDate && s.Sales_Date < endDate)
                .SumAsync(s => (decimal?)s.Final_Total) ?? 0;

            // Total purchases for the day
            var totalPurchases = await _context.PurchaseInvoices
                .Where(p => p.Purchase_Date >= startDate && p.Purchase_Date < endDate)
                .SumAsync(p => (decimal?)p.Total_Cost) ?? 0;

            var report = new DailyReportDto
            {
                Date = startDate,
                TotalSales = totalSales,
                TotalPurchases = totalPurchases,
                Profit = totalSales - totalPurchases
            };

            return Ok(report);
        }

        /// <summary>
        /// Get monthly financial report
        /// </summary>
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyReport(int year, int month)
        {
            var startDate = new DateTime(year, month, 1).ToUniversalTime();
            var endDate = startDate.AddMonths(1);

            // Total sales for the month
            var totalSales = await _context.SalesInvoices
                .Where(s => s.Sales_Date >= startDate && s.Sales_Date < endDate)
                .SumAsync(s => (decimal?)s.Final_Total) ?? 0;

            // Total purchases for the month
            var totalPurchases = await _context.PurchaseInvoices
                .Where(p => p.Purchase_Date >= startDate && p.Purchase_Date < endDate)
                .SumAsync(p => (decimal?)p.Total_Cost) ?? 0;

            var report = new MonthlyReportDto
            {
                Year = year,
                Month = month,
                MonthName = startDate.ToString("MMMM"),
                TotalSales = totalSales,
                TotalPurchases = totalPurchases,
                Profit = totalSales - totalPurchases
            };

            return Ok(report);
        }

        /// <summary>
        /// Get yearly financial report
        /// </summary>
        [HttpGet("yearly")]
        public async Task<IActionResult> GetYearlyReport(int year)
        {
            var startDate = new DateTime(year, 1, 1).ToUniversalTime();
            var endDate = startDate.AddYears(1);

            // Total sales for the year
            var totalSales = await _context.SalesInvoices
                .Where(s => s.Sales_Date >= startDate && s.Sales_Date < endDate)
                .SumAsync(s => (decimal?)s.Final_Total) ?? 0;

            // Total purchases for the year
            var totalPurchases = await _context.PurchaseInvoices
                .Where(p => p.Purchase_Date >= startDate && p.Purchase_Date < endDate)
                .SumAsync(p => (decimal?)p.Total_Cost) ?? 0;

            var report = new YearlyReportDto
            {
                Year = year,
                TotalSales = totalSales,
                TotalPurchases = totalPurchases,
                Profit = totalSales - totalPurchases
            };

            return Ok(report);
        }
    }
}