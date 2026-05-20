using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerSelfRegisterController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public CustomerSelfRegisterController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Register a new customer with vehicles
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CustomerRegisterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email already registered" });

            // Create User (Identity)
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                Created_At = DateTime.UtcNow
            };

            // Hash password
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create Customer record
            var customer = new Customer
            {
                User_Id = user.Id,
                Pending_Credit = 0,
                Credit_Due_Date = null,
                Total_Spent = 0
            };

            _context.Customers.Add(customer);

            // Assign "Customer" role in database (role ID is "3" as seeded in AppDbContext)
            var userRole = new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = "3"
            };
            _context.UserRoles.Add(userRole);

            await _context.SaveChangesAsync();

            // Add vehicles if provided
            if (request.Vehicles != null && request.Vehicles.Any())
            {
                foreach (var v in request.Vehicles)
                {
                    var vehicle = new Vehicle
                    {
                        Customer_ID = customer.Customer_ID,
                        Reg_Number = v.VehicleNumber,
                        Make = v.Make,
                        Model = v.Model,
                        Manufacture_Year = v.Year,
                        Vehicle_Type = "Car",
                        Fuel_Type = "Petrol",
                        Condition = "Good",
                        Usage_Pattern = "Regular",
                        Created_At = DateTime.UtcNow
                    };
                    _context.Vehicles.Add(vehicle);
                }
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                message = "Registration successful",
                customerId = customer.Customer_ID,
                fullName = user.FullName,
                email = user.Email
            });
        }

        /// <summary>
        /// Customer login
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CustomerLoginRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Invalid email or password" });

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.User_Id == user.Id);
            if (customer == null)
                return Unauthorized(new { message = "Customer account not found" });

            var token = GenerateJwtToken(user, customer.Customer_ID);

            return Ok(new
            {
                message = "Login successful",
                token = token,
                customerId = customer.Customer_ID,
                fullName = user.FullName,
                email = user.Email
            });
        }

        /// <summary>
        /// Get customer profile with vehicles
        /// </summary>
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var customerId = GetCustomerIdFromToken();
            if (customerId == null)
                return Unauthorized(new { message = "Invalid token" });

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Customer_ID == customerId);

            if (customer == null)
                return NotFound(new { message = "Customer not found" });

            var user = await _context.Users.FindAsync(customer.User_Id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var vehicles = await _context.Vehicles
                .Where(v => v.Customer_ID == customer.Customer_ID)
                .ToListAsync();

            var profile = new CustomerProfileResponseDto
            {
                CustomerId = customer.Customer_ID,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address ?? string.Empty,
                PendingCredit = customer.Pending_Credit,
                Vehicles = vehicles.Select(v => new CustomerVehicleResponseDto
                {
                    VehicleId = v.Vehicle_ID,
                    VehicleNumber = v.Reg_Number,
                    Make = v.Make,
                    Model = v.Model,
                    Year = v.Manufacture_Year,
                    Color = "Not specified"
                }).ToList()
            };

            return Ok(profile);
        }

        /// <summary>
        /// Add a new vehicle to customer profile
        /// </summary>
        [Authorize]
        [HttpPost("add-vehicle")]
        public async Task<IActionResult> AddVehicle([FromBody] AddVehicleRequestDto request)
        {
            var customerId = GetCustomerIdFromToken();
            if (customerId == null)
                return Unauthorized(new { message = "Invalid token" });

            var existingVehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Reg_Number == request.VehicleNumber);

            if (existingVehicle != null)
                return BadRequest(new { message = "Vehicle number already registered" });

            var vehicle = new Vehicle
            {
                Customer_ID = customerId.Value,
                Reg_Number = request.VehicleNumber,
                Make = request.Make,
                Model = request.Model,
                Manufacture_Year = request.Year,
                Vehicle_Type = "Car",
                Fuel_Type = "Petrol",
                Condition = "Good",
                Usage_Pattern = "Regular",
                Created_At = DateTime.UtcNow
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Vehicle added successfully", vehicleId = vehicle.Vehicle_ID });
        }

        /// <summary>
        /// Delete a vehicle from customer profile
        /// </summary>
        [Authorize]
        [HttpDelete("delete-vehicle/{vehicleId}")]
        public async Task<IActionResult> DeleteVehicle(int vehicleId)
        {
            var customerId = GetCustomerIdFromToken();
            if (customerId == null)
                return Unauthorized(new { message = "Invalid token" });

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Vehicle_ID == vehicleId && v.Customer_ID == customerId);

            if (vehicle == null)
                return NotFound(new { message = "Vehicle not found" });

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Vehicle deleted successfully" });
        }

        /// <summary>
        /// Update customer profile
        /// </summary>
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var customerId = GetCustomerIdFromToken();
            if (customerId == null)
                return Unauthorized(new { message = "Invalid token" });

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Customer_ID == customerId);

            if (customer == null)
                return NotFound(new { message = "Customer not found" });

            var user = await _context.Users.FindAsync(customer.User_Id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            if (!string.IsNullOrEmpty(request.Address))
                user.Address = request.Address;

            if (!string.IsNullOrEmpty(request.Password))
            {
                var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully" });
        }

        /// <summary>
        /// Generate JWT Token for customer
        /// </summary>
        private string GenerateJwtToken(User user, int customerId)
        {
            // Get JWT settings from appsettings.json (JwtSettings section)
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expiryMinutes = Convert.ToDouble(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "120");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(ClaimTypes.Role, "Customer"),
                new Claim("CustomerId", customerId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int? GetCustomerIdFromToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var claim = identity?.FindFirst("CustomerId");
            if (claim != null && int.TryParse(claim.Value, out int customerId))
                return customerId;
            return null;
        }
    }

    // Request DTO for update profile
    public class UpdateProfileRequest
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Password { get; set; }
    }
}