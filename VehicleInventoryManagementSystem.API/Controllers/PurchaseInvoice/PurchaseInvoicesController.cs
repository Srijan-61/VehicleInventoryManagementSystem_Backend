using Microsoft.AspNetCore.Mvc;
using VehicleInventoryManagementSystem.Application.DTOs.PurchaseInvoice;
using VehicleInventoryManagementSystem.Application.Interfaces.PurchaseInvoice;

namespace VehicleInventoryManagementSystem.API.Controllers.PurchaseInvoice
{
    [ApiController]
    [Route("api/purchase-invoices")]
    // TODO: [Authorize(Roles = "Admin")] once JWT is wired up.
    public class PurchaseInvoicesController : ControllerBase
    {
        private readonly IPurchaseInvoiceService _service;

        public PurchaseInvoicesController(IPurchaseInvoiceService service)
        {
            _service = service;
        }

        /// <summary>F4 — Admin creates a purchase invoice and updates stock.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(PurchaseInvoiceResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreatePurchaseInvoiceRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _service.CreateAsync(request, cancellationToken);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Invoice!.PurchaseInvoiceNo },
                result.Invoice);
        }

        /// <summary>List all purchase invoices (newest first).</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<PurchaseInvoiceSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var invoices = await _service.GetAllAsync(cancellationToken);
            return Ok(invoices);
        }

        /// <summary>View one purchase invoice with line items.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PurchaseInvoiceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var invoice = await _service.GetByIdAsync(id, cancellationToken);
            if (invoice is null)
            {
                return NotFound(new { error = $"Purchase invoice {id} not found." });
            }
            return Ok(invoice);
        }
    }
}