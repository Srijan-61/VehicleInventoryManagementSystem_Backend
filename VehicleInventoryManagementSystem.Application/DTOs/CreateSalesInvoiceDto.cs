using System.ComponentModel.DataAnnotations;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class CreateSalesInvoiceDto
    {
        [Required]
        public int Customer_ID { get; set; }

        [Required]
        public int Staff_ID { get; set; }

        public bool Is_Paid { get; set; }

        public List<CreateSalesItemDto> Items { get; set; } = new();
    }

    public class CreateSalesItemDto
    {
        [Required]
        public int Part_ID { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity_Sold { get; set; }
    }
}