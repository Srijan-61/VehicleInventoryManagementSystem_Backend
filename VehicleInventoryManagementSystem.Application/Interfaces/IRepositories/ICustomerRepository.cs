using System;
using System.Collections.Generic;
using System.Text;
using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface ICustomerRepository
    {
        Task AddCustomerAsync(ICustomerRepository customer);
        Task SaveChangesAsync();

        Task<IEnumerable<ICustomerRepository>> GetCustomersWithUsersAsync();
    }
}
