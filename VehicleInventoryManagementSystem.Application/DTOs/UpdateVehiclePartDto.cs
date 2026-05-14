using System.ComponentModel.DataAnnotations;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class UpdateVehiclePartDto
    {
        [Required]
        [StringLength(100)]
        public string Part_Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Part_Category { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock_Quantity { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Minimum_Stock_Level { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Unit_Price { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Purchase_Price { get; set; }
    }
}