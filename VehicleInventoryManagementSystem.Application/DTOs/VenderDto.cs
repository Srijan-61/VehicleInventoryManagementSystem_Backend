using System.ComponentModel.DataAnnotations;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class VendorDto 
    { 
        public int Id { get; set; } 
        public string Name { get; set; } 
        public string Phone { get; set; } 
        public string Email { get; set; } 
        public string Address { get; set; } 
    }
    
    public class CreateUpdateVendorDto 
    { 
        public string Name { get; set; } 
        public string Phone { get; set; } 
        public string Email { get; set; } 
        public string Address { get; set; } 
    }
}