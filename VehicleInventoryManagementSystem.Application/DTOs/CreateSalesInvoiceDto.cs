using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class CreateSalesInvoiceDto
    {
        [Required]
        public int Customer_ID { get; set; }

        [Required]
        public int Staff_ID { get; set; }

        [Required]
        public bool Is_Paid { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one part must be sold.")]
        public List<SalesItemDto> Items { get; set; } = new List<SalesItemDto>();
    }
}
