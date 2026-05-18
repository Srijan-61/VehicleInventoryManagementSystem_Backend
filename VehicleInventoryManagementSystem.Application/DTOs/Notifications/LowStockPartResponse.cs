namespace VehicleInventoryManagementSystem.Application.DTOs.Notifications
{
    /// <summary>
    /// One part flagged as low stock.
    /// </summary>
    public class LowStockPartResponse
    {
        public int PartId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string PartCategory { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int MinimumStockLevel { get; set; }

        /// <summary>
        /// True when stock has fallen to zero. Useful for highlighting
        /// "out of stock" vs merely "low" in a UI.
        /// </summary>
        public bool IsOutOfStock => StockQuantity <= 0;
    }
}