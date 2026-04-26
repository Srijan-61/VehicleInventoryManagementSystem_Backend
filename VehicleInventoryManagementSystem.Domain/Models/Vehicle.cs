using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class Vehicle
    {
        [Key]
        public int Vehicle_ID { get; set; }

        [ForeignKey(nameof(Customer))]
        public int Customer_ID { get; set; }

        public string Reg_Number { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Manufacture_Year { get; set; }
        public string Vehicle_Type { get; set; }
        public string Fuel_Type { get; set; }
        public string Condition { get; set; }
        public string Usage_Pattern { get; set; }
        public DateTime Created_At { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
