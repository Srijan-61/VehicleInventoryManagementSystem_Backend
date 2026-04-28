using System;
using System.Collections.Generic;
using System.Text;
using VehicleInventoryManagementSystem.Application.DTOs.Auth;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDto request);
    }
}
