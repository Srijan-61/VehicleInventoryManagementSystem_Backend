using System;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    public class CustomerFeatureService : ICustomerFeatureService
    {
        private readonly ICustomerFeatureRepository _repository;

        public CustomerFeatureService(ICustomerFeatureRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerHistoryResultDto> GetCustomerHistoryAsync(string userId)
        {
            var customerId = await _repository.GetCustomerIdByUserIdAsync(userId);
            if (customerId == null)
            {
                throw new Exception("Customer profile not found for the logged-in user.");
            }

            var historyItems = await _repository.GetCustomerHistoryAsync(customerId.Value);

            return new CustomerHistoryResultDto
            {
                HistoryItems = historyItems
            };
        }
    }
}
