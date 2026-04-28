namespace VehicleInventoryManagementSystem.Application.DTOs.Notifications
{
    /// <summary>
    /// Result envelope for the low-stock report. Wrapping the list with
    /// summary fields lets the client show "12 parts low" without
    /// recomputing on every render.
    /// </summary>
    public class LowStockReportResponse
    {
        public int Threshold { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public DateTime GeneratedAt { get; set; }
        public List<LowStockPartResponse> Items { get; set; } = new();
    }
}