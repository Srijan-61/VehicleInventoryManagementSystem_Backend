using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class PartRequest
    {
        [Key]
        public int Request_ID { get; set; }

        [ForeignKey(nameof(Customer))]
        public int Customer_ID { get; set; }

        public string Requested_Part_Name { get; set; }
        public int Requested_Quantity { get; set; }
        public string Status { get; set; }
        public DateTime Request_Date { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
