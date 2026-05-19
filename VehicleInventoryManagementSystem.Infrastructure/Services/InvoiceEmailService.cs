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
                                    <tr style='border-bottom: 1px solid #eeeeee;'>
                                        <td style='padding: 12px; text-align: left; color: #333333;'>{WebUtility.HtmlEncode(item.PartName)}</td>
                                        <td style='padding: 12px; text-align: left; color: #666666;'>{WebUtility.HtmlEncode(item.Brand)}</td>
                                        <td style='padding: 12px; text-align: center; color: #333333;'>{item.Quantity_Sold}</td>
                                        <td style='padding: 12px; text-align: right; color: #333333;'>${item.Unit_Price:N2}</td>
                                        <td style='padding: 12px; text-align: right; font-weight: bold; color: #111111;'>${item.Total_Price:N2}</td>
                                    </tr>");
            }

            var paymentStatus = invoice.Is_Paid ?
                "<span style='background-color: #d4edda; color: #155724; padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: bold; display: inline-block;'>PAID</span>" :
                "<span style='background-color: #fff3cd; color: #856404; padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: bold; display: inline-block;'>PENDING CREDIT</span>";

            var dateStr = invoice.Sales_Date.ToString("MMM dd, yyyy");

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Invoice #{invoice.Sales_Invoice_No}</title>
</head>
<body style='margin: 0; padding: 0; font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7f6; color: #333333; line-height: 1.6;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f7f6; padding: 40px 20px;'>
        <tr>
            <td align='center'>
                <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05); max-width: 600px; margin: 0 auto;'>
                    
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%); padding: 30px 40px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; letter-spacing: 1px;'>SALES INVOICE</h1>
                            <p style='color: #a8c1f0; margin: 5px 0 0 0; font-size: 14px;'>Vehicle Inventory Management System</p>
                        </td>
                    </tr>

                    <!-- Customer & Invoice Details -->
                    <tr>
                        <td style='padding: 30px 40px;'>
                            <table width='100%' cellpadding='0' cellspacing='0'>
                                <tr>
                                    <td width='50%' valign='top'>
                                        <p style='margin: 0 0 5px 0; font-size: 12px; color: #888888; text-transform: uppercase; font-weight: bold;'>Billed To:</p>
                                        <p style='margin: 0; font-size: 16px; font-weight: bold; color: #222222;'>{WebUtility.HtmlEncode(invoice.CustomerName)}</p>
                                    </td>
                                    <td width='50%' valign='top' align='right'>
                                        <p style='margin: 0 0 5px 0; font-size: 12px; color: #888888; text-transform: uppercase; font-weight: bold;'>Invoice Details:</p>
                                        <p style='margin: 0 0 3px 0; font-size: 14px;'><strong>No:</strong> #{invoice.Sales_Invoice_No}</p>
                                        <p style='margin: 0 0 3px 0; font-size: 14px;'><strong>Date:</strong> {dateStr}</p>
                                        <p style='margin: 0; font-size: 14px;'><strong>Status:</strong> {paymentStatus}</p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- Items Table -->
                    <tr>
                        <td style='padding: 0 40px 20px 40px;'>
                            <table width='100%' cellpadding='0' cellspacing='0' style='border-collapse: collapse;'>
                                <thead>
                                    <tr style='background-color: #f8f9fa; border-bottom: 2px solid #e9ecef;'>
                                        <th style='padding: 12px; text-align: left; font-size: 13px; color: #666666; text-transform: uppercase;'>Part</th>
                                        <th style='padding: 12px; text-align: left; font-size: 13px; color: #666666; text-transform: uppercase;'>Brand</th>
                                        <th style='padding: 12px; text-align: center; font-size: 13px; color: #666666; text-transform: uppercase;'>Qty</th>
                                        <th style='padding: 12px; text-align: right; font-size: 13px; color: #666666; text-transform: uppercase;'>Price</th>
                                        <th style='padding: 12px; text-align: right; font-size: 13px; color: #666666; text-transform: uppercase;'>Total</th>
                                    </tr>
                                </thead>
                                <tbody>
{itemsHtml}
                                </tbody>
                            </table>
                        </td>
                    </tr>

                    <!-- Summary Totals -->
                    <tr>
                        <td style='padding: 0 40px 30px 40px;'>
                            <table width='100%' cellpadding='0' cellspacing='0'>
                                <tr>
                                    <td width='50%' valign='top'>
                                        <p style='margin: 0; font-size: 13px; color: #888888;'>Processed by: {WebUtility.HtmlEncode(invoice.StaffName)}</p>
                                    </td>
                                    <td width='50%' valign='top'>
                                        <table width='100%' cellpadding='0' cellspacing='0' style='border-collapse: collapse;'>
                                            <tr>
                                                <td style='padding: 8px 12px; text-align: right; color: #666666; font-size: 14px;'>Sub Total:</td>
                                                <td style='padding: 8px 12px; text-align: right; font-weight: bold; font-size: 14px;'>${invoice.Sub_Total:N2}</td>
                                            </tr>
                                            <tr>
                                                <td style='padding: 8px 12px; text-align: right; color: #e53e3e; font-size: 14px;'>Discount:</td>
                                                <td style='padding: 8px 12px; text-align: right; font-weight: bold; color: #e53e3e; font-size: 14px;'>-${invoice.Discount_Amount:N2}</td>
                                            </tr>
                                            <tr style='border-top: 2px solid #e9ecef;'>
                                                <td style='padding: 15px 12px 8px 12px; text-align: right; color: #111111; font-size: 18px; font-weight: bold;'>Final Total:</td>
                                                <td style='padding: 15px 12px 8px 12px; text-align: right; color: #1e3c72; font-size: 18px; font-weight: bold;'>${invoice.Final_Total:N2}</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f8f9fa; padding: 20px 40px; text-align: center; border-top: 1px solid #e9ecef;'>
                            <p style='margin: 0 0 10px 0; font-size: 14px; color: #555555; font-weight: bold;'>Thank you for your business!</p>
                            <p style='margin: 0; font-size: 12px; color: #888888;'>If you have any questions regarding this invoice, please contact our support team.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}