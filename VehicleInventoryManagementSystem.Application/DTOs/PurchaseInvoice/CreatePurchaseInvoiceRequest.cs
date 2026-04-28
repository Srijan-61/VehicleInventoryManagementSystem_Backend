using System.ComponentModel.DataAnnotations;

namespace VehicleInventoryManagementSystem.Application.DTOs.PurchaseInvoice
{
    /// <summary>
    /// Payload accepted by POST /api/purchase-invoices.
    /// </summary>
    public class CreatePurchaseInvoiceRequest
    {
        [Required(ErrorMessage = "Vendor ID is required.")]
        public int VendorId { get; set; }

        // TODO: derive from JWT claims once authentication is wired up.
        [Required(ErrorMessage = "Admin ID is required.")]
        public int AdminId { get; set; }

        [Required(ErrorMessage = "Purchase date is required.")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Payment status is required.")]
        [RegularExpression("^(Paid|Unpaid|Partial)$",
            ErrorMessage = "Payment status must be Paid, Unpaid, or Partial.")]
        public string PaymentStatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "At least one purchase item is required.")]
        [MinLength(1, ErrorMessage = "Invoice must contain at least one item.")]
        public List<PurchaseItemRequest> Items { get; set; } = new();
    }
}