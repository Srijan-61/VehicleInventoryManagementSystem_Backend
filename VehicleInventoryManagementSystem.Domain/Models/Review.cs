using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class Review
    {
        [Key]
        public int Review_ID { get; set; }

        [Required]
        public int Customer_ID { get; set; }

        [Required]
        public int Appointment_ID { get; set; }

        [Required]
        public int Rating { get; set; }

        public string Comment { get; set; }

        [Required]
        public DateTime Review_Date { get; set; }

        [ForeignKey("Customer_ID")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("Appointment_ID")]
        public virtual Appointment Appointment { get; set; }
    }
}
