using System.ComponentModel.DataAnnotations;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class CreatePurchaseInvoiceRequestDto
    {
        [Required]
        public int VendorId { get; set; }

        [Required]
        public int AdminId { get; set; }

        [Required]
        [MinLength(1)]
        public List<PurchaseInvoiceItemDto> Items { get; set; } = new();

        public string? Notes { get; set; }
    }

    public class PurchaseInvoiceItemDto
    {
        [Required]
        public int PartId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }

    public class PurchaseInvoiceResponseDto
    {
        public int PurchaseInvoiceNo { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalCost { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public List<PurchaseInvoiceItemResponseDto> Items { get; set; } = new();
    }

    public class PurchaseInvoiceItemResponseDto
    {
        public string PartName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}