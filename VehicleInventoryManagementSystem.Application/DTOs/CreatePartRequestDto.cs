using System.ComponentModel.DataAnnotations;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class CreatePartRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Requested_Part_Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 100)]
        public int Requested_Quantity { get; set; }
    }
}