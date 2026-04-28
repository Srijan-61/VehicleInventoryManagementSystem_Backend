using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IRepositories
{
    public interface IStaffRepository
    {
        Task AddStaffAsync(Staff staff);
        Task SaveChangesAsync();
    }
}
