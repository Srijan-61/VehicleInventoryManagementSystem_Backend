namespace VehicleInventoryManagementSystem.Application.DTOs
{
    // Used to show customer's vehicles in appointment dropdown.
    public class CustomerVehicleListDto
    {
        public int Vehicle_ID { get; set; }
        public string Reg_Number { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Vehicle_Type { get; set; } = string.Empty;
    }

    // Used to show customer's appointment history and completed appointments.
    public class CustomerAppointmentListDto
    {
        public int Appointment_ID { get; set; }
        public int Vehicle_ID { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public DateTime Appointment_Date { get; set; }
        public string Service_Type { get; set; } = string.Empty;
        public string Appointment_Status { get; set; } = string.Empty;
    }

    // Used to show customer's unavailable part request history.
    public class CustomerPartRequestListDto
    {
        public int Request_ID { get; set; }
        public string Requested_Part_Name { get; set; } = string.Empty;
        public int Requested_Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Request_Date { get; set; }
    }

    // Used to show customer's submitted reviews.
    public class CustomerReviewListDto
    {
        public int Review_ID { get; set; }
        public int Appointment_ID { get; set; }
        public string Service_Type { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime Review_Date { get; set; }
    }
}