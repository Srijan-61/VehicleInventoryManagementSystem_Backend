using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class Alert
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PartId { get; set; }

        [Required]
        [MaxLength(50)]
        public string AlertType { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        public int? StockQuantity { get; set; }

        public bool IsResolved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ResolvedAt { get; set; }

        [ForeignKey("PartId")]
        public virtual VehiclePart? Part { get; set; }
    }
}