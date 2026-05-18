namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class LowStockAlertDto
    {
        public int PartId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime AlertTime { get; set; }
    }

    public class OverdueCreditDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal PendingCredit { get; set; }
        public int DaysOverdue { get; set; }
    }
}