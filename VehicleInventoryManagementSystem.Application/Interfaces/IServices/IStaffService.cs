using System;
using System.Collections.Generic;
using System.Text;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.DTOs.Auth;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    public interface IStaffService
    {
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterStaffAsync(RegisterStaffDto dto);

        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterCustomerWithVehicleAsync(RegisterCustomerWithVehicleDto dto);

        Task<(bool Succeeded, SalesInvoiceResultDto? Data, IEnumerable<string> Errors)> CreateSalesInvoiceAsync(CreateSalesInvoiceDto dto);

        Task<IEnumerable<CustomerDropdownDto>> GetCustomersForDropdownAsync();
        Task<int> GetCurrentStaffIdAsync(string userId);

    }
}
