namespace VehicleInventoryManagementSystem.Application.DTOs.PurchaseInvoice
{
    public class PurchaseItemResponse
    {
        public int PartId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public int QuantityPurchased { get; set; }
        public decimal PurchaseUnitCost { get; set; }
        public decimal LineTotal { get; set; }
    }
}