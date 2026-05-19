using System.ComponentModel.DataAnnotations;

namespace VehicleInventoryManagementSystem.Application.DTOs.Invoices
{
    public class SendInvoiceEmailRequestDto
    {
        [Required]
        public int Customer_ID { get; set; }

        [Required]
        public int Sales_Invoice_No { get; set; }
    }
}