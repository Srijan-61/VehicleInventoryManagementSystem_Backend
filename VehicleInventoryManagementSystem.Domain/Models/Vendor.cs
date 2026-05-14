using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class Vendor
    {
        [Key]
        public int Vendor_ID { get; set; }

        public string Vendor_Name { get; set; }
        public string Vendor_Contact { get; set; }
        public string Vendor_Email { get; set; }
        public string Vendor_Address { get; set; }
        public DateTime Created_At { get; set; }
    }
}
