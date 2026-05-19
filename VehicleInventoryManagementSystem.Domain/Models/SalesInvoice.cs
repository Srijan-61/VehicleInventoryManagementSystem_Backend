using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class SalesInvoice
    {
        [Key]
        public int Sales_Invoice_No { get; set; }

        [ForeignKey(nameof(Customer))]
        public int Customer_ID { get; set; }

        [ForeignKey(nameof(Staff))]
        public int Staff_ID { get; set; }

        public DateTime Sales_Date { get; set; }
        public decimal Sub_Total { get; set; }
        public decimal Discount_Amount { get; set; }
        public decimal Final_Total { get; set; }
        public bool Is_Paid { get; set; }
        public DateTime? Credit_Due_Date { get; set; }
        public DateTime Created_At { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Staff Staff { get; set; }
    }
}