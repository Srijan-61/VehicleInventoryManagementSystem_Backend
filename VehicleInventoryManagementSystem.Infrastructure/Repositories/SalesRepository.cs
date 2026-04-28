using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    public class SalesRepository : ISalesRepository
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public SalesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Customer_ID == customerId);
        }

        public async Task<VehiclePart?> GetPartByIdAsync(int partId)
        {
            return await _context.VehicleParts.FirstOrDefaultAsync(p => p.Part_ID == partId);
        }

        public async Task AddSalesInvoiceAsync(SalesInvoice invoice)
        {
            await _context.SalesInvoices.AddAsync(invoice);
        }

        public async Task AddSalesItemAsync(SalesItem item)
        {
            await _context.SalesItems.AddAsync(item);
        }

        // Starts transaction because invoice, items, stock and customer credit update together.
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}