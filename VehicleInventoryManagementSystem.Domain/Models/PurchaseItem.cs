using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class PurchaseItem
    {
        [Key, Column(Order = 0)]
        [ForeignKey(nameof(PurchaseInvoice))]
        public int Purchase_Invoice_No { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey(nameof(VehiclePart))]
        public int Part_ID { get; set; }

        public int Quantity_Purchased { get; set; }
        public decimal Purchase_Unit_Cost { get; set; }
        public decimal Line_Total { get; set; }

        public virtual PurchaseInvoice PurchaseInvoice { get; set; }
        public virtual VehiclePart VehiclePart { get; set; }
    }
}
