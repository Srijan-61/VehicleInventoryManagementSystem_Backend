using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class PurchaseInvoice
    {
        [Key]
        public int Purchase_Invoice_No { get; set; }

        [ForeignKey(nameof(Vendor))]
        public int Vendor_ID { get; set; }

        [ForeignKey(nameof(Admin))]
        public int Admin_ID { get; set; }

        // New fields for invoice tracking
        [Required]
        public string Invoice_Number { get; set; } = string.Empty;

        public DateTime Purchase_Date { get; set; }

        // Financial fields
        public decimal Sub_Total { get; set; }
        public decimal Tax_Amount { get; set; }
        public decimal Total_Cost { get; set; }

        // Status and notes
        public string Payment_Status { get; set; } = "Pending";
        public string? Notes { get; set; }

        public DateTime Created_At { get; set; }

        // Navigation properties
        public virtual Vendor Vendor { get; set; }
        public virtual Admin Admin { get; set; }
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }
}