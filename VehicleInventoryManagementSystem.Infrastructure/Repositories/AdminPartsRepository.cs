using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    public class AdminPartsRepository : IAdminPartsRepository
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public AdminPartsRepository(AppDbContext context)
        {
            _context = context;
        }

        // Gets all vehicle parts for admin inventory view.
        public async Task<List<VehiclePart>> GetAllPartsAsync()
        {
            return await _context.VehicleParts
                .OrderBy(p => p.Part_Name)
                .ToListAsync();
        }

        // Gets a single part before edit, delete, or purchase update.
        public async Task<VehiclePart?> GetPartByIdAsync(int partId)
        {
            return await _context.VehicleParts
                .FirstOrDefaultAsync(p => p.Part_ID == partId);
        }

        // Checks vendor before creating purchase invoice.
        public async Task<bool> VendorExistsAsync(int vendorId)
        {
            return await _context.Vendors.AnyAsync(v => v.Vendor_ID == vendorId);
        }

        // Checks admin before assigning purchase invoice.
        public async Task<bool> AdminExistsAsync(int adminId)
        {
            return await _context.Admins.AnyAsync(a => a.Admin_ID == adminId);
        }

        public async Task AddPurchaseInvoiceAsync(PurchaseInvoice invoice)
        {
            await _context.PurchaseInvoices.AddAsync(invoice);
        }

        public async Task AddPurchaseItemAsync(PurchaseItem item)
        {
            await _context.PurchaseItems.AddAsync(item);
        }

        // Purchase invoice and stock update must be saved together.
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