namespace VehicleInventoryManagementSystem.Application.DTOs.PurchaseInvoice
{
    public class PurchaseInvoiceSummaryResponse
    {
        public int PurchaseInvoiceNo { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public decimal TotalCost { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }
}