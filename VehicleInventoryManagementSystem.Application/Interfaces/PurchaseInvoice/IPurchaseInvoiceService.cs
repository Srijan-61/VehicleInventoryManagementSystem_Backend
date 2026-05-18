using VehicleInventoryManagementSystem.Application.DTOs.PurchaseInvoice;

namespace VehicleInventoryManagementSystem.Application.Interfaces.PurchaseInvoice
{
    /// <summary>
    /// Manages purchase invoices and the resulting stock updates (Feature F4).
    /// </summary>
    public interface IPurchaseInvoiceService
    {
        Task<PurchaseInvoiceResultDto> CreateAsync(
            CreatePurchaseInvoiceRequest request,
            CancellationToken cancellationToken = default);

        Task<List<PurchaseInvoiceSummaryResponse>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<PurchaseInvoiceResponse?> GetByIdAsync(
            int purchaseInvoiceNo,
            CancellationToken cancellationToken = default);
    }
}