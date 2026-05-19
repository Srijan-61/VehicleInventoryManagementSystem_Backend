using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class RegisterCustomerWithVehicleDto
    {
        // Customer User Details
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        
        public string Password { get; set; } = string.Empty;

        // Vehicle Details
        [Required]
        public string Reg_Number { get; set; } = string.Empty;
        [Required]
        public string Make { get; set; } = string.Empty;
        [Required]
        public string Model { get; set; } = string.Empty;
        public int Manufacture_Year { get; set; }
        public string Vehicle_Type { get; set; } = string.Empty;
        public string Fuel_Type { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public string Usage_Pattern { get; set; } = string.Empty;
    }
}
