using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class SalesItemDto
    {
        [Required]
        public int Part_ID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }

    public class CustomerDropdownDto
    {
        public int Customer_ID { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
