using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class SalesInvoiceResultDto
    {
        public int Invoice_No { get; set; }
        public decimal Sub_Total { get; set; }
        public decimal Discount_Amount { get; set; }
        public decimal Final_Total { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<SalesItemResultDto> Items { get; set; } = new();
    }

    // The individual items dto for the response
    public class SalesItemResultDto
    {
        public int Part_ID { get; set; }
        public string Part_Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Unit_Price { get; set; }
        public decimal Total_Price { get; set; }
    }
}
