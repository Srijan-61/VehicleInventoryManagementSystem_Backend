using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.DTOs;

namespace VehicleInventoryManagementSystem.Application.Interfaces.IServices
{
    

    public interface ICustomerDetailsService
    {
        Task<CustomerDetailsDto> GetCustomerDetailsAsync(int customerId);
    }
}
