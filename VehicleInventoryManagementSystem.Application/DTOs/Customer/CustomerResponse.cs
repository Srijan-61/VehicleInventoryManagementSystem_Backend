namespace VehicleInventoryManagementSystem.Application.DTOs.Customer
{
    /// <summary>
    /// Safe customer view returned to clients. Excludes password hashes,
    /// security stamps, and other sensitive Identity fields.
    /// </summary>
    public class CustomerResponse
    {
        public int CustomerId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}