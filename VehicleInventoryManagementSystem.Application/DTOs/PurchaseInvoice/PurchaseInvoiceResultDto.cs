namespace VehicleInventoryManagementSystem.Application.DTOs.PurchaseInvoice
{
    public class PurchaseInvoiceResultDto
    {
        public bool Succeeded { get; set; }
        public PurchaseInvoiceResponse? Invoice { get; set; }
        public List<string> Errors { get; set; } = new();

        public static PurchaseInvoiceResultDto Success(PurchaseInvoiceResponse invoice) =>
            new() { Succeeded = true, Invoice = invoice };

        public static PurchaseInvoiceResultDto Failure(IEnumerable<string> errors) =>
            new() { Succeeded = false, Errors = errors.ToList() };
    }
}