using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class Customer
    {
        [Key]
        public int Customer_ID { get; set; }

        [ForeignKey(nameof(User))]
        public string User_Id { get; set; }

        public decimal Pending_Credit { get; set; }
        public DateTime? Credit_Due_Date { get; set; }
        public decimal Total_Spent { get; set; }

        public virtual User User { get; set; }
    }
}
