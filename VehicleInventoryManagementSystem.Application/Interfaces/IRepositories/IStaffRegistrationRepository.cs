using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    // Feature 2 staff registration 
    public interface IStaffRegistrationRepository
    {
        Task AddStaffAsync(Staff staff);
        Task SaveChangesAsync();
    }
}
