using System;
using System.Collections.Generic;
using System.Text;
using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface IStaffService
    {
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterStaffAsync(RegisterStaffDto dto);
    }
}
