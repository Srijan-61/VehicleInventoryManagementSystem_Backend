using System.Collections.Generic;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class CustomerSearchResultDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string VehicleRegNumber { get; set; }
        public string VehicleModel { get; set; }
    }
}
