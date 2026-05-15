using Microsoft.EntityFrameworkCore;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Domain.Models;
using VehicleInventoryManagementSystem.Infrastructure.Presistance;

namespace VehicleInventoryManagementSystem.Infrastructure.Repositories
{
    // This repository deals with all the database work for the sales feature (Features 7 and 16)
    // It handles parts lookup, stock updates, saving invoices, and fetching customers and staff info
    public class SalesFeatureRepository : ISalesFeatureRepository
    {
        private readonly AppDbContext _context;

        public SalesFeatureRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<VehiclePart?> GetPartByIdAsync(int partId)
        {
            return await _context.VehicleParts.FindAsync(partId);
        }

        public void UpdatePart(VehiclePart part)
        {
            _context.VehicleParts.Update(part);
        }

        public async Task AddInvoiceAsync(SalesInvoice invoice)
        {
            await _context.SalesInvoices.AddAsync(invoice);
        }

        public async Task AddSalesItemsAsync(IEnumerable<SalesItem> items)
        {
            await _context.SalesItems.AddRangeAsync(items);
        }

        public async Task<Staff?> GetStaffByUserIdAsync(string userId)
        {
            return await _context.StaffProfiles
                .FirstOrDefaultAsync(s => s.User_Id == userId);
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithUsersAsync()
        {
            return await _context.Customers.Include(c => c.User).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        Task<IEnumerable<ICustomerRepository>> ISalesFeatureRepository.GetCustomersWithUsersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
