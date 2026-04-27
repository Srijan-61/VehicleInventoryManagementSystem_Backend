using System;
using System.Collections.Generic;
using System.Text;
using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface IStaffService
    {
        Task<bool> RegisterStaffAsync(RegisterStaffDto dto);
    }
}
