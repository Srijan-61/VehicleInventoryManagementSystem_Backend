using System.ComponentModel.DataAnnotations;

namespace VehicleInventoryManagementSystem.Application.DTOs.Invoices
{
    public class SendInvoiceEmailRequestDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Valid sales invoice number is required.")]
        public int Sales_Invoice_No { get; set; }
    }
}