using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    // Repository interface for staff registration (Feature 2)
    // Defines the database operations needed to save a new staff profile
    public interface IStaffRegistrationRepository
    {
        Task AddStaffAsync(Staff staff);
        Task SaveChangesAsync();
    }
}
