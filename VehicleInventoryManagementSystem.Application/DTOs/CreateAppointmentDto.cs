using System.ComponentModel.DataAnnotations;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class CreateAppointmentDto
    {
        [Required]
        public int Vehicle_ID { get; set; }

        [Required]
        public DateTime Appointment_Date { get; set; }

        [Required]
        [StringLength(100)]
        public string Service_Type { get; set; } = string.Empty;
    }
}