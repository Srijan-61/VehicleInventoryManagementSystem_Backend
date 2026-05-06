using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface IInvoiceEmailService
    {
        Task SendInvoiceEmailAsync(int salesInvoiceNo);
    }
}