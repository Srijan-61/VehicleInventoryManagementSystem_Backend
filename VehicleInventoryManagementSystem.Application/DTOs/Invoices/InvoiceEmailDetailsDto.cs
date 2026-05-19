namespace VehicleInventoryManagementSystem.Application.DTOs.Invoices
{
    public class InvoiceEmailDetailsDto
    {
        public int Sales_Invoice_No { get; set; }

        public int Customer_ID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }

        public string StaffName { get; set; } = string.Empty;

        public DateTime Sales_Date { get; set; }
        public decimal Sub_Total { get; set; }
        public decimal Discount_Amount { get; set; }
        public decimal Final_Total { get; set; }
        public bool Is_Paid { get; set; }
        public DateTime? Credit_Due_Date { get; set; }

        public List<InvoiceEmailItemDto> Items { get; set; } = new();
    }

    public class InvoiceEmailItemDto
    {
        public string PartName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public int Quantity_Sold { get; set; }
        public decimal Unit_Price { get; set; }
        public decimal Total_Price { get; set; }
    }

    public class CustomerInvoiceDropdownDto
    {
        public int Sales_Invoice_No { get; set; }
        public DateTime Sales_Date { get; set; }
        public decimal Final_Total { get; set; }
        public bool Is_Paid { get; set; }
    }
}