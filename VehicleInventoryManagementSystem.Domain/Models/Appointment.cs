using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class Appointment
    {
        [Key]
        public int Appointment_ID { get; set; }

        [ForeignKey(nameof(Vehicle))]
        public int Vehicle_ID { get; set; }

        public DateTime Appointment_Date { get; set; }
        public string Service_Type { get; set; }
        public string Appointment_Status { get; set; }
        public DateTime Created_At { get; set; }

        public virtual Vehicle Vehicle { get; set; }
    }
}
