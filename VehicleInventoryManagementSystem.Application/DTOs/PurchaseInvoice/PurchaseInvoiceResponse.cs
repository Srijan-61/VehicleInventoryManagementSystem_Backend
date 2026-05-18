namespace VehicleInventoryManagementSystem.Application.DTOs.PurchaseInvoice
{
    public class PurchaseInvoiceResponse
    {
        public int PurchaseInvoiceNo { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public int AdminId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalCost { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<PurchaseItemResponse> Items { get; set; } = new();
    }
}