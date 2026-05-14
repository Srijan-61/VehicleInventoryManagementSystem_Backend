using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    public class CustomerSearchService : ICustomerSearchService
    {
        private readonly ICustomerSearchRepository _repository;

        public CustomerSearchService(ICustomerSearchRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CustomerSearchResultDto>> SearchAsync(string searchTerm)
        {
            var results = await _repository.SearchCustomersAsync(searchTerm);

            return results.Select(data => new CustomerSearchResultDto
            {
                CustomerId = data.Customer.Customer_ID,
                Name = data.Customer.User?.FullName,
                Phone = data.Customer.User?.PhoneNumber,
                VehicleRegNumber = data.PrimaryVehicle?.Reg_Number ?? "No Vehicle",
                VehicleModel = data.PrimaryVehicle?.Model ?? "N/A"
            }).ToList();
        }
    }
}
