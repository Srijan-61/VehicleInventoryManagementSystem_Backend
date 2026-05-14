using System.ComponentModel.DataAnnotations;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class CustomerDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Vehicle { get; set; }
        public string Model { get; set; }
        public List<string> History { get; set; } = new List<string>();
    }
}