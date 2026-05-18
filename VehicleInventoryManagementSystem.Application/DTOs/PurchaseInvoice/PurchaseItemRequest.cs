using System.ComponentModel.DataAnnotations;

namespace VehicleInventoryManagementSystem.Application.DTOs.PurchaseInvoice
{
    /// <summary>
    /// One line item in a purchase invoice request.
    /// </summary>
    public class PurchaseItemRequest
    {
        [Required(ErrorMessage = "Part ID is required.")]
        public int PartId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int QuantityPurchased { get; set; }

        [Required(ErrorMessage = "Unit cost is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit cost must be greater than 0.")]
        public decimal PurchaseUnitCost { get; set; }
    }
}