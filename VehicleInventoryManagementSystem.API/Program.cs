using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using VehicleInventoryManagementSystem.Application.Interfaces.Customer;
using VehicleInventoryManagementSystem.Application.Interfaces.Notifications;
using VehicleInventoryManagementSystem.Application.Interfaces.PurchaseInvoice;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;
using VehicleInventoryManagementSystem.Infrastructure.Services.Customer;
using VehicleInventoryManagementSystem.Infrastructure.Services.Notifications;
using VehicleInventoryManagementSystem.Infrastructure.Services.PurchaseInvoice;
using System.Security.Claims;
using System.Text;
using VehicleInventoryManagementSystem.Application.DTOs.Auth;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Infrastructure.Repositories;
using VehicleInventoryManagementSystem.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//// 1. Database setup
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity Configuration
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// F12 � Customer registration
builder.Services.AddScoped<VehicleInventoryManagementSystem.Infrastructure.Services.CustomerRegistrationService>();

// F4 � Purchase invoices
builder.Services.AddScoped<IPurchaseInvoiceService, PurchaseInvoiceService>();

// F15 � Notifications (low-stock alerts; email reminders to follow)
builder.Services.AddScoped<INotificationService, NotificationService>();
// Bind JwtSettings to the DI container
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();


// Register Services & Repositories
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ISalesRepository, SalesRepository>();
builder.Services.AddScoped<IVehiclePartRepository, VehiclePartRepository>();



builder.Services.AddScoped<IStaffReportRepository, StaffReportRepository>();
builder.Services.AddScoped<IStaffReportService, StaffReportService>();

builder.Services.Configure<SmtpEmailSettings>(
    builder.Configuration.GetSection("SmtpEmailSettings"));

builder.Services.AddScoped<IInvoiceEmailRepository, InvoiceEmailRepository>();
builder.Services.AddScoped<IInvoiceEmailService, InvoiceEmailService>();
builder.Services.AddScoped<IEmailSenderService, SmtpEmailSenderService>();

builder.Services.AddScoped<ICustomerSelfRepository, CustomerSelfRepository>();
builder.Services.AddScoped<ICustomerSelfService, CustomerSelfService>();

builder.Services.AddScoped<IAdminPartsRepository, AdminPartsRepository>();
builder.Services.AddScoped<IAdminPartsService, AdminPartsService>();

builder.Services.AddScoped<IStaffRegistrationRepository, StaffRegistrationRepository>();
builder.Services.AddScoped<IStaffRegistrationService, StaffRegistrationService>();

builder.Services.AddScoped<IVendorManagementRepository, VendorManagementRepository>();
builder.Services.AddScoped<IVendorManagementService, VendorManagementService>();

builder.Services.AddScoped<ICustomerDetailsRepository, CustomerDetailsRepository>();
builder.Services.AddScoped<ICustomerDetailsService, CustomerDetailsService>();

builder.Services.AddScoped<ICustomerSearchRepository, CustomerSearchRepository>();
builder.Services.AddScoped<ICustomerSearchService, CustomerSearchService>();

builder.Services.AddScoped<ICustomerRegistrationRepository, CustomerRegistrationRepository>();
builder.Services.AddScoped<VehicleInventoryManagementSystem.Application.Interfaces.Customer.ICustomerRegistrationService, VehicleInventoryManagementSystem.Infrastructure.Services.Customer.CustomerRegistrationService>();
builder.Services.AddScoped<ISalesFeatureRepository, SalesFeatureRepository>();
builder.Services.AddScoped<ISalesFeatureService, SalesFeatureService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.Name
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:1234") //  frontend URL
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Useful for auth tokens/cookies
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();