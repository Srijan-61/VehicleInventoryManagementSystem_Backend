namespace VehicleInventoryManagementSystem.Application.DTOs.Customer
{
    /// <summary>
    /// Service-layer outcome for a registration attempt.
    /// The controller maps Succeeded -> 201 and !Succeeded -> 400.
    /// </summary>
    public class RegistrationResultDto
    {
        public bool Succeeded { get; set; }
        public CustomerResponse? Customer { get; set; }
        public List<string> Errors { get; set; } = new();

        public static RegistrationResultDto Success(CustomerResponse customer) =>
            new() { Succeeded = true, Customer = customer };

        public static RegistrationResultDto Failure(IEnumerable<string> errors) =>
            new() { Succeeded = false, Errors = errors.ToList() };
    }
}