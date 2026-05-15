using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    public class CustomerDetailsService : ICustomerDetailsService
    {
        private readonly ICustomerDetailsRepository _repository;

        public CustomerDetailsService(ICustomerDetailsRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerDetailsDto> GetCustomerDetailsAsync(int customerId)
        {
            var (customer, vehicle, history) = await _repository.GetCustomerDataAsync(customerId);

            if (customer == null) return null;

            var dto = new CustomerDetailsDto
            {
                Id = customer.Customer_ID,
                Name = customer.User?.FullName,
                Phone = customer.User?.PhoneNumber,
                
                Vehicle = vehicle?.Reg_Number,
                Model = vehicle?.Model,
                
                History = history
            };

            return dto;
        }
    }
}
