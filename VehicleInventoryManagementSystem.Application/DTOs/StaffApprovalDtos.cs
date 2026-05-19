namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class StaffAppointmentApprovalListDto
    {
        public int Appointment_ID { get; set; }
        public int Vehicle_ID { get; set; }
        public int Customer_ID { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public DateTime Appointment_Date { get; set; }
        public string Service_Type { get; set; } = string.Empty;
        public string Appointment_Status { get; set; } = string.Empty;
    }

    public class StaffPartRequestApprovalListDto
    {
        public int Request_ID { get; set; }
        public int Customer_ID { get; set; }
        public string Requested_Part_Name { get; set; } = string.Empty;
        public int Requested_Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Request_Date { get; set; }
    }

    public class StaffRejectDto
    {
        public string? Reason { get; set; }
    }
}