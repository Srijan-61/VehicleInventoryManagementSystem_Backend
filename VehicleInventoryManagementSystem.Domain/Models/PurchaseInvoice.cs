using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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

        public DateTime Purchase_Date { get; set; }
        public decimal Total_Cost { get; set; }
        public string Payment_Status { get; set; }
        public DateTime Created_At { get; set; }

        public virtual Vendor Vendor { get; set; }
        public virtual Admin Admin { get; set; }
    }
}
