using System;
using System.Collections.Generic;
using System.Text;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    public class SalesRepository(AppDbContext _context) : ISalesRepository
    {
        public async Task AddInvoiceAsync(SalesInvoice invoice) => await _context.SalesInvoices.AddAsync(invoice);
        public async Task AddSalesItemsAsync(IEnumerable<SalesItem> items) => await _context.SalesItems.AddRangeAsync(items);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
