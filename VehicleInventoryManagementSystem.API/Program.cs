using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Security.Claims;
using System.Text;
using VehicleInventoryManagementSystem.Application.DTOs.Auth;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;
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

// Bind JwtSettings to the DI container
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();


// 3. Register Services & Repositories
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ISalesRepository, SalesRepository>();
builder.Services.AddScoped<IVehiclePartRepository, VehiclePartRepository>();



builder.Services.AddScoped<ICustomerSelfRepository, CustomerSelfRepository>();
builder.Services.AddScoped<ISalesRepository, SalesRepository>();

builder.Services.AddScoped<ICustomerSelfService, CustomerSelfService>();

builder.Services.AddScoped<IAdminPartsRepository, AdminPartsRepository>();
builder.Services.AddScoped<IAdminPartsService, AdminPartsService>();

// Feature 2: Staff Registration (Vertical Slice)
builder.Services.AddScoped<IStaffRegistrationRepository, StaffRegistrationRepository>();
builder.Services.AddScoped<IStaffRegistrationService, StaffRegistrationService>();

// Feature 6: Customer Registration (Vertical Slice)
builder.Services.AddScoped<ICustomerRegistrationRepository, CustomerRegistrationRepository>();
builder.Services.AddScoped<ICustomerRegistrationService, CustomerRegistrationService>();

// Features 7 & 16: Sales & POS (Vertical Slice)
builder.Services.AddScoped<ISalesFeatureRepository, SalesFeatureRepository>();
builder.Services.AddScoped<ISalesFeatureService, SalesFeatureService>();

// 4. Configure Authentication & JWT
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
