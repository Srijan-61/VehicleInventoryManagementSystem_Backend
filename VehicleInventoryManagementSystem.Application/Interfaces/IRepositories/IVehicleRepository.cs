using System;
using System.Collections.Generic;
using System.Text;
using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface IVehicleRepository
    {
        Task AddVehicleAsync(Vehicle vehicle);
        Task SaveChangesAsync();
    }
}