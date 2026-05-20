using System.ComponentModel.DataAnnotations;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class CustomerReviewDto
    {
        [Required]
        public int Appointment_ID { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; } = string.Empty;
    }
}