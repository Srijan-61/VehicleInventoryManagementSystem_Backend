using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class VehiclePart
    {
        [Key]
        public int Part_ID { get; set; }

        public string Part_Name { get; set; }
        public string Part_Category { get; set; }
        public string Brand { get; set; }
        public int Stock_Quantity { get; set; }
        public int Minimum_Stock_Level { get; set; }
        public decimal Unit_Price { get; set; }
        public decimal Purchase_Price { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
}
