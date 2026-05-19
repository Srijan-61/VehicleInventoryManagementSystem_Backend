using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    public class AlertService : IAlertService
    {
        private readonly AppDbContext _context;
        private readonly IEmailSenderService _emailService;
        private readonly ILogger<AlertService> _logger;

        public AlertService(
            AppDbContext context,
            IEmailSenderService emailService,
            ILogger<AlertService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<List<LowStockAlertDto>> GetLowStockAlertsAsync()
        {
            var lowStockParts = await _context.VehicleParts
                .Where(p => p.Stock_Quantity < 10 && p.IsAvailable)
                .Select(p => new LowStockAlertDto
                {
                    PartId = p.Part_ID,
                    PartName = p.Part_Name,
                    CurrentStock = p.Stock_Quantity,
                    Message = $"Part '{p.Part_Name}' has only {p.Stock_Quantity} units remaining",
                    AlertTime = DateTime.UtcNow
                })
                .ToListAsync();

            return lowStockParts;
        }

        public async Task<List<OverdueCreditDto>> GetOverdueCreditsAsync(int daysOverdue = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysOverdue);

            var overdueCustomers = await (from customer in _context.Customers
                                          join user in _context.Users on customer.User_Id equals user.Id
                                          where customer.Pending_Credit > 0 &&
                                                customer.Credit_Due_Date != null &&
                                                customer.Credit_Due_Date < cutoffDate
                                          select new OverdueCreditDto
                                          {
                                              CustomerId = customer.Customer_ID,
                                              CustomerName = user.FullName,
                                              Email = user.Email ?? string.Empty,
                                              PendingCredit = customer.Pending_Credit,
                                              DaysOverdue = (DateTime.UtcNow - customer.Credit_Due_Date.Value).Days
                                          }).ToListAsync();

            return overdueCustomers;
        }

        public async Task CheckAndCreateLowStockAlertsAsync()
        {
            var lowStockParts = await _context.VehicleParts
                .Where(p => p.Stock_Quantity < 10 && p.IsAvailable)
                .ToListAsync();

            foreach (var part in lowStockParts)
            {
                var existingAlert = await _context.Set<Alert>()
                    .FirstOrDefaultAsync(a => a.PartId == part.Part_ID &&
                        a.AlertType == "LowStock" &&
                        a.CreatedAt > DateTime.UtcNow.AddHours(-24));

                if (existingAlert == null)
                {
                    var alert = new Alert
                    {
                        PartId = part.Part_ID,
                        AlertType = "LowStock",
                        Message = $"Low stock: '{part.Part_Name}' has {part.Stock_Quantity} units left",
                        StockQuantity = part.Stock_Quantity,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Set<Alert>().Add(alert);
                    await SendLowStockEmailToAdmins(part);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Low stock check completed at {Time}", DateTime.UtcNow);
        }

        public async Task CheckAndSendCreditRemindersAsync()
        {
            var overdueCustomers = await GetOverdueCreditsAsync(30);

            foreach (var customer in overdueCustomers)
            {
                var existingReminder = await _context.Set<CreditReminder>()
                    .FirstOrDefaultAsync(r => r.CustomerId == customer.CustomerId &&
                        r.SentDate > DateTime.UtcNow.AddDays(-7));

                if (existingReminder == null)
                {
                    await SendCreditReminderToCustomerAsync(customer.CustomerId);
                }
            }
        }

        public async Task<bool> SendCreditReminderToCustomerAsync(int customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Customer_ID == customerId);

            if (customer == null || customer.Pending_Credit <= 0)
                return false;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == customer.User_Id);

            if (user == null || string.IsNullOrEmpty(user.Email))
                return false;

            var subject = $"Payment Reminder: Overdue Credit of NPR {customer.Pending_Credit:N2}";
            var body = $@"
                <h2>Overdue Credit Payment Reminder</h2>
                <p>Dear {user.FullName},</p>
                <p>Your credit balance of <b>NPR {customer.Pending_Credit:N2}</b> is overdue.</p>
                <p>Please make the payment at your earliest convenience.</p>
                <hr>
                <p>Best regards,<br>Vehicle Service Center Team</p>
            ";

            try
            {
                await _emailService.SendEmailAsync(user.Email, subject, body);

                var reminder = new CreditReminder
                {
                    CustomerId = customerId,
                    Amount = customer.Pending_Credit,
                    SentDate = DateTime.UtcNow,
                    ReminderCount = 1
                };
                _context.Set<CreditReminder>().Add(reminder);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", user.Email);
                return false;
            }
        }

        public async Task<bool> SendAllOverdueRemindersAsync()
        {
            var overdueCustomers = await GetOverdueCreditsAsync(30);
            var successCount = 0;

            foreach (var customer in overdueCustomers)
            {
                if (await SendCreditReminderToCustomerAsync(customer.CustomerId))
                    successCount++;
            }

            _logger.LogInformation("Sent {SuccessCount} reminders out of {TotalCount}", successCount, overdueCustomers.Count);
            return successCount > 0;
        }

        private async Task SendLowStockEmailToAdmins(VehiclePart part)
        {
            var admins = await _context.Users.ToListAsync();
            var subject = $"LOW STOCK ALERT: {part.Part_Name}";
            var body = $@"
                <h2>Low Stock Alert</h2>
                <p>Part: <b>{part.Part_Name}</b></p>
                <p>Current Stock: <b style='color:red'>{part.Stock_Quantity} units</b></p>
                <p>Threshold: 10 units</p>
                <p>Please restock immediately.</p>
            ";

            foreach (var admin in admins)
            {
                if (!string.IsNullOrEmpty(admin.Email))
                {
                    try
                    {
                        await _emailService.SendEmailAsync(admin.Email, subject, body);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send low stock email to {Email}", admin.Email);
                    }
                }
            }
        }
    }
}