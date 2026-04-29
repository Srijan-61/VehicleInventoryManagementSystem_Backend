using System;
using System.Collections.Generic;
using System.Text;
using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface IVehiclePartRepository
    {
        Task<VehiclePart?> GetByIdAsync(int partId);
        void Update(VehiclePart part);
        Task SaveChangesAsync();
    }
}
