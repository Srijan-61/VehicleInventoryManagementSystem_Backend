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

    // Used when purchasing a brand-new part that does not yet exist in inventory.
    // The backend creates the VehiclePart record first, then creates the purchase invoice.
    public class CreateNewPartPurchaseDto
    {
        [Required]
        public int Vendor_ID { get; set; }

        [Required]
        public string Payment_Status { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Part_Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Part_Category { get; set; } = string.Empty;

        // Selling price (shown to customers on invoices)
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Selling_Price { get; set; }

        [Required]
        [Range(1, 10000)]
        public int Quantity_Purchased { get; set; }

        // What the admin paid per unit to the vendor
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Purchase_Unit_Cost { get; set; }
    }
}