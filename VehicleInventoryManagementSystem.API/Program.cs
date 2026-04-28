using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using VehicleInventoryManagementSystem.Application.Interfaces.Customer;
using VehicleInventoryManagementSystem.Application.Interfaces.Notifications;
using VehicleInventoryManagementSystem.Application.Interfaces.PurchaseInvoice;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;
using VehicleInventoryManagementSystem.Infrastructure.Services.Customer;
using VehicleInventoryManagementSystem.Infrastructure.Services.Notifications;
using VehicleInventoryManagementSystem.Infrastructure.Services.PurchaseInvoice;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity Configuration
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// F12 — Customer registration
builder.Services.AddScoped<ICustomerRegistrationService, CustomerRegistrationService>();

// F4 — Purchase invoices
builder.Services.AddScoped<IPurchaseInvoiceService, PurchaseInvoiceService>();

// F15 — Notifications (low-stock alerts; email reminders to follow)
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();