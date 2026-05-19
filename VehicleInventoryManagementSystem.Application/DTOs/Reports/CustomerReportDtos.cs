namespace VehicleInventoryManagementSystem.Application.DTOs.Reports
{
    public class RegularCustomerReportDto
    {
        public int Customer_ID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int TotalPurchases { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime LastPurchaseDate { get; set; }
    }

    public class HighSpenderReportDto
    {
        public int Customer_ID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalInvoices { get; set; }
        public DateTime LastPurchaseDate { get; set; }
    }

    public class PendingCreditReportDto
    {
        public int Customer_ID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int Sales_Invoice_No { get; set; }
        public decimal PendingAmount { get; set; }
        public DateTime? Credit_Due_Date { get; set; }
        public bool IsOverdue { get; set; }
    }
}