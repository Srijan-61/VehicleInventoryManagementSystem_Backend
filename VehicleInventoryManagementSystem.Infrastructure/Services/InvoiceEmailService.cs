using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using VehicleInventoryManagementSystem.Application.DTOs.Invoices;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    /// <summary>
    /// Handles invoice email business logic and validation.
    /// </summary>
    public class InvoiceEmailService : IInvoiceEmailService
    {
        private readonly IInvoiceEmailRepository _invoiceEmailRepository;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ILogger<InvoiceEmailService> _logger;

        public InvoiceEmailService(
            IInvoiceEmailRepository invoiceEmailRepository,
            IEmailSenderService emailSenderService,
            ILogger<InvoiceEmailService> logger)
        {
            _invoiceEmailRepository = invoiceEmailRepository;
            _emailSenderService = emailSenderService;
            _logger = logger;
        }

        /// <summary>
        /// Returns all invoices of selected customer.
        /// </summary>
        public async Task<List<CustomerInvoiceDropdownDto>> GetInvoicesByCustomerAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("Valid customer ID is required.");

            return await _invoiceEmailRepository.GetInvoicesByCustomerAsync(customerId);
        }

        /// <summary>
        /// Returns full invoice details for preview.
        /// </summary>
        public async Task<InvoiceEmailDetailsDto?> GetInvoiceEmailDetailsAsync(
            int customerId,
            int salesInvoiceNo)
        {
            if (customerId <= 0)
                throw new ArgumentException("Valid customer ID is required.");

            if (salesInvoiceNo <= 0)
                throw new ArgumentException("Valid sales invoice number is required.");

            return await _invoiceEmailRepository.GetInvoiceEmailDetailsAsync(
                customerId,
                salesInvoiceNo
            );
        }

        /// <summary>
        /// Sends invoice email to customer.
        /// </summary>
        public async Task SendInvoiceEmailAsync(SendInvoiceEmailRequestDto request)
        {
            if (request.Customer_ID <= 0)
                throw new ArgumentException("Valid customer ID is required.");

            if (request.Sales_Invoice_No <= 0)
                throw new ArgumentException("Valid sales invoice number is required.");

            var invoice = await _invoiceEmailRepository.GetInvoiceEmailDetailsAsync(
                request.Customer_ID,
                request.Sales_Invoice_No
            );

            if (invoice == null)
                throw new KeyNotFoundException(
                    "Invoice was not found for the selected customer."
                );

            if (string.IsNullOrWhiteSpace(invoice.CustomerEmail))
                throw new InvalidOperationException(
                    "Customer email address is missing."
                );

            if (invoice.Items.Count == 0)
                throw new InvalidOperationException(
                    "Invoice has no items to send."
                );

            var subject = $"Sales Invoice #{invoice.Sales_Invoice_No}";
            var body = BuildInvoiceEmailBody(invoice);

            _logger.LogInformation(
                "Sending invoice email for invoice no {InvoiceNo} to customer {CustomerId}.",
                request.Sales_Invoice_No,
                request.Customer_ID
            );

            await _emailSenderService.SendEmailAsync(
                invoice.CustomerEmail,
                subject,
                body
            );
        }

        /// <summary>
        /// Builds professional HTML invoice email body.
        /// </summary>
        private static string BuildInvoiceEmailBody(InvoiceEmailDetailsDto invoice)
        {
            var itemsHtml = new StringBuilder();

            foreach (var item in invoice.Items)
            {
                itemsHtml.Append($@"
                    <tr>
                        <td>{WebUtility.HtmlEncode(item.PartName)}</td>
                        <td>{WebUtility.HtmlEncode(item.Brand)}</td>
                        <td>{item.Quantity_Sold}</td>
                        <td>{item.Unit_Price:N2}</td>
                        <td>{item.Total_Price:N2}</td>
                    </tr>");
            }

            var paymentStatus = invoice.Is_Paid ? "Paid" : "Pending Credit";

            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; color: #333;'>

                    <h2>Vehicle Parts Sales Invoice</h2>

                    <p>Dear {WebUtility.HtmlEncode(invoice.CustomerName)},</p>

                    <p>
                        Thank you for choosing our vehicle service center.
                        Your invoice details are listed below.
                    </p>

                    <hr/>

                    <p><strong>Invoice No:</strong> {invoice.Sales_Invoice_No}</p>
                    <p><strong>Sales Date:</strong> {invoice.Sales_Date:yyyy-MM-dd}</p>
                    <p><strong>Processed By:</strong> {WebUtility.HtmlEncode(invoice.StaffName)}</p>
                    <p><strong>Payment Status:</strong> {paymentStatus}</p>

                    <br/>

                    <table border='1'
                           cellpadding='8'
                           cellspacing='0'
                           style='border-collapse: collapse; width: 100%;'>

                        <thead style='background-color:#f2f2f2;'>
                            <tr>
                                <th>Part</th>
                                <th>Brand</th>
                                <th>Quantity</th>
                                <th>Unit Price</th>
                                <th>Total</th>
                            </tr>
                        </thead>

                        <tbody>
                            {itemsHtml}
                        </tbody>
                    </table>

                    <br/>

                    <h3>Invoice Summary</h3>

                    <p><strong>Sub Total:</strong> {invoice.Sub_Total:N2}</p>
                    <p><strong>Discount:</strong> {invoice.Discount_Amount:N2}</p>
                    <p><strong>Final Total:</strong> {invoice.Final_Total:N2}</p>

                    <hr/>

                    <p>
                        If you have any questions regarding this invoice,
                        please contact our support team.
                    </p>

                    <p>
                        Thank you for your business.
                    </p>

                </body>
                </html>";
        }
    }
}