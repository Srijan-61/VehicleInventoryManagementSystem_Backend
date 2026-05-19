using System.ComponentModel.DataAnnotations;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class CreatePurchaseDto
    {
        [Required]
        public int Vendor_ID { get; set; }

        [Required]
        public string Payment_Status { get; set; } = string.Empty;

        [Required]
        public List<CreatePurchaseItemDto> Items { get; set; } = new();
    }

    public class CreatePurchaseItemDto
    {
        [Required]
        public int Part_ID { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity_Purchased { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Purchase_Unit_Cost { get; set; }
    }
}