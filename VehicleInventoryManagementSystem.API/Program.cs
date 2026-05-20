using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using VehicleInventoryManagementSystem.Application.DTOs.Auth;
using VehicleInventoryManagementSystem.Application.Interfaces;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;
using VehicleInventoryManagementSystem.Infrastructure.Repositories;
using VehicleInventoryManagementSystem.Infrastructure.Services;
using VehicleInventoryManagementSystem.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database setup
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity configuration
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Core auth service
builder.Services.AddScoped<IAuthService, AuthService>();

// Common repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ISalesRepository, SalesRepository>();
builder.Services.AddScoped<IVehiclePartRepository, VehiclePartRepository>();

// Staff reports
builder.Services.AddScoped<IStaffReportRepository, StaffReportRepository>();
builder.Services.AddScoped<IStaffReportService, StaffReportService>();

// SMTP and invoice email
builder.Services.Configure<SmtpEmailSettings>(
    builder.Configuration.GetSection("SmtpEmailSettings"));

builder.Services.AddScoped<IInvoiceEmailRepository, InvoiceEmailRepository>();
builder.Services.AddScoped<IInvoiceEmailService, InvoiceEmailService>();
builder.Services.AddScoped<IEmailSenderService, SmtpEmailSenderService>();

// Customer self-service
builder.Services.AddScoped<ICustomerSelfRepository, CustomerSelfRepository>();
builder.Services.AddScoped<ICustomerSelfService, CustomerSelfService>();

// Admin parts management
builder.Services.AddScoped<IAdminPartsRepository, AdminPartsRepository>();
builder.Services.AddScoped<IAdminPartsService, AdminPartsService>();

// Staff registration
builder.Services.AddScoped<IStaffRegistrationRepository, StaffRegistrationRepository>();
builder.Services.AddScoped<IStaffRegistrationService, StaffRegistrationService>();

// Vendor management
builder.Services.AddScoped<IVendorManagementRepository, VendorManagementRepository>();
builder.Services.AddScoped<IVendorManagementService, VendorManagementService>();

// Customer details and search
builder.Services.AddScoped<ICustomerDetailsRepository, CustomerDetailsRepository>();
builder.Services.AddScoped<ICustomerDetailsService, CustomerDetailsService>();

builder.Services.AddScoped<ICustomerSearchRepository, CustomerSearchRepository>();
builder.Services.AddScoped<ICustomerSearchService, CustomerSearchService>();

// Customer registration
builder.Services.AddScoped<ICustomerRegistrationRepository, CustomerRegistrationRepository>();
builder.Services.AddScoped<ICustomerRegistrationService, CustomerRegistrationService>();

// Sales feature
builder.Services.AddScoped<ISalesFeatureRepository, SalesFeatureRepository>();
builder.Services.AddScoped<ISalesFeatureService, SalesFeatureService>();

// Alert services
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddHostedService<AlertMonitorBackgroundService>();

// Authentication and JWT
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
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
        ),
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.Name
    };
});

// CORS configuration - Allow both frontend ports
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Global Exception Handler
app.UseMiddleware<VehicleInventoryManagementSystem.API.Middlewares.GlobalExceptionMiddleware>();

//app.UseHttpsRedirection();

// Use CORS - THIS MUST BE BEFORE Authentication
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();