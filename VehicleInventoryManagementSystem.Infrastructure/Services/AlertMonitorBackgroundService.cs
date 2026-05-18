using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VehicleInventoryManagementSystem.Application.Interfaces;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    public class AlertMonitorBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AlertMonitorBackgroundService> _logger;

        public AlertMonitorBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<AlertMonitorBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Alert Monitor Background Service is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

                    // Check low stock alerts
                    await alertService.CheckAndCreateLowStockAlertsAsync();

                    // Check credit reminders  
                    await alertService.CheckAndSendCreditRemindersAsync();

                    _logger.LogInformation("Alert monitoring cycle completed at {Time}", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during alert monitoring");
                }

                // Wait 6 hours before next check
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }

            _logger.LogInformation("Alert Monitor Background Service is stopping");
        }
    }
}