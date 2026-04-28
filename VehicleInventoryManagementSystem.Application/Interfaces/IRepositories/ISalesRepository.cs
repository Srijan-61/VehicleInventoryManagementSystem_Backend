using System;
using System.Collections.Generic;
using System.Text;
using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface ISalesRepository
    {
        Task AddInvoiceAsync(SalesInvoice invoice);
        Task AddSalesItemsAsync(IEnumerable<SalesItem> items);
        Task SaveChangesAsync();
    }
}
