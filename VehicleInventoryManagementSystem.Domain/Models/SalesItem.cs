using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class SalesItem
    {
        [Key, Column(Order = 0)]
        [ForeignKey(nameof(SalesInvoice))]
        public int Sales_Invoice_No { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey(nameof(VehiclePart))]
        public int Part_ID { get; set; }

        public int Quantity_Sold { get; set; }
        public decimal Unit_Price { get; set; }
        public decimal Total_Price { get; set; }

        public virtual SalesInvoice SalesInvoice { get; set; }
        public virtual VehiclePart VehiclePart { get; set; }
    }
}
