using Microsoft.Extensions.Logging;
using VehicleInventoryManagementSystem.Application.DTOs;
using VehicleInventoryManagementSystem.Application.Interfaces.IRepositories;
using VehicleInventoryManagementSystem.Application.Interfaces.IServices;
using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Infrastructure.Services
{
    public class CustomerSelfService : ICustomerSelfService
    {
        private readonly ICustomerSelfRepository _customerRepository;
        private readonly ILogger<CustomerSelfService> _logger;

        public CustomerSelfService(
            ICustomerSelfRepository customerRepository,
            ILogger<CustomerSelfService> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<string> BookAppointmentAsync(CreateAppointmentDto dto)
        {
            if (dto.Appointment_Date <= DateTime.UtcNow)
                return "Appointment date must be in the future.";

            if (string.IsNullOrWhiteSpace(dto.Service_Type))
                return "Service type is required.";

            var vehicle = await _customerRepository.GetCustomerVehicleAsync(dto.Customer_ID, dto.Vehicle_ID);

            if (vehicle == null)
                return "Vehicle not found for this customer.";

            var appointment = new Appointment
            {
                Vehicle_ID = dto.Vehicle_ID,
                Appointment_Date = dto.Appointment_Date,
                Service_Type = dto.Service_Type.Trim(),
                Appointment_Status = "Pending",
                Created_At = DateTime.UtcNow
            };

            await _customerRepository.AddAppointmentAsync(appointment);
            await _customerRepository.SaveChangesAsync();

            _logger.LogInformation("Appointment booked for customer {CustomerId}.", dto.Customer_ID);

            return "Appointment booked successfully.";
        }

        public async Task<string> RequestUnavailablePartAsync(CreatePartRequestDto dto)
        {
            var customerExists = await _customerRepository.CustomerExistsAsync(dto.Customer_ID);

            if (!customerExists)
                return "Customer not found.";

            if (string.IsNullOrWhiteSpace(dto.Requested_Part_Name))
                return "Requested part name is required.";

            if (dto.Requested_Quantity <= 0)
                return "Requested quantity must be greater than zero.";

            var existingPart = await _customerRepository.GetPartByNameAsync(dto.Requested_Part_Name);

            if (existingPart != null && existingPart.IsAvailable && existingPart.Stock_Quantity > 0)
                return "This part is currently available in stock.";

            var partRequest = new PartRequest
            {
                Customer_ID = dto.Customer_ID,
                Requested_Part_Name = dto.Requested_Part_Name.Trim(),
                Requested_Quantity = dto.Requested_Quantity,
                Status = "Pending",
                Request_Date = DateTime.UtcNow
            };

            await _customerRepository.AddPartRequestAsync(partRequest);
            await _customerRepository.SaveChangesAsync();

            _logger.LogInformation("Part request created for customer {CustomerId}.", dto.Customer_ID);

            return "Unavailable part request submitted successfully.";
        }

        public async Task<string> SubmitReviewAsync(CreateReviewDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                return "Rating must be between 1 and 5.";

            if (string.IsNullOrWhiteSpace(dto.Comment))
                return "Review comment is required.";

            var appointment = await _customerRepository
                .GetCustomerAppointmentAsync(dto.Customer_ID, dto.Appointment_ID);

            if (appointment == null)
                return "Appointment not found for this customer.";

            var alreadyReviewed = await _customerRepository
                .ReviewExistsForAppointmentAsync(dto.Appointment_ID);

            if (alreadyReviewed)
                return "This appointment has already been reviewed.";

            var review = new Review
            {
                Customer_ID = dto.Customer_ID,
                Appointment_ID = dto.Appointment_ID,
                Rating = dto.Rating,
                Comment = dto.Comment.Trim(),
                Review_Date = DateTime.UtcNow
            };

            await _customerRepository.AddReviewAsync(review);
            await _customerRepository.SaveChangesAsync();

            _logger.LogInformation("Review submitted for appointment {AppointmentId}.", dto.Appointment_ID);

            return "Review submitted successfully.";
        }
    }
}