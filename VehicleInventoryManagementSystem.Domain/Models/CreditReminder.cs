using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class CreditReminder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime SentDate { get; set; } = DateTime.UtcNow;

        public bool IsPaid { get; set; } = false;

        public DateTime? PaidDate { get; set; }

        public int ReminderCount { get; set; } = 1;

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
    }
}