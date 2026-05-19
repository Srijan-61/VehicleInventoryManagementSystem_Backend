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

        // Books a new appointment for the logged-in customer.
        public async Task<string> BookAppointmentAsync(
            CustomerAppointmentDto dto,
            string userId)
        {
            var customerId = await GetLoggedInCustomerIdAsync(userId);

            if (customerId == null)
                return "Customer profile not found.";

            if (dto.Appointment_Date <= DateTime.UtcNow)
                return "Appointment date must be in the future.";

            if (string.IsNullOrWhiteSpace(dto.Service_Type))
                return "Service type is required.";

            var vehicle = await _customerRepository.GetCustomerVehicleAsync(
                customerId.Value,
                dto.Vehicle_ID
            );

            if (vehicle == null)
                return "Vehicle not found for this customer.";

            var slotExists = await _customerRepository
                .AppointmentSlotExistsAsync(dto.Appointment_Date);

            if (slotExists)
                return "This appointment slot is already booked.";

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

            _logger.LogInformation(
                "Appointment booked for customer {CustomerId}.",
                customerId.Value
            );

            return "Appointment request submitted successfully. Please wait for staff approval.";
        }

        // Creates unavailable part request for the logged-in customer.
        public async Task<string> RequestUnavailablePartAsync(
            CustomerPartRequestDto dto,
            string userId)
        {
            var customerId = await GetLoggedInCustomerIdAsync(userId);

            if (customerId == null)
                return "Customer profile not found.";

            if (string.IsNullOrWhiteSpace(dto.Requested_Part_Name))
                return "Requested part name is required.";

            if (dto.Requested_Quantity <= 0)
                return "Requested quantity must be greater than zero.";

            var existingPart = await _customerRepository
                .GetPartByNameAsync(dto.Requested_Part_Name);

            if (existingPart != null &&
                existingPart.IsAvailable &&
                existingPart.Stock_Quantity > 0)
            {
                return "This part is currently available in stock.";
            }

            var duplicateRequest = await _customerRepository
                .ActivePartRequestExistsAsync(
                    customerId.Value,
                    dto.Requested_Part_Name
                );

            if (duplicateRequest)
                return "You already have a pending request for this part.";

            var partRequest = new PartRequest
            {
                Customer_ID = customerId.Value,
                Requested_Part_Name = dto.Requested_Part_Name.Trim(),
                Requested_Quantity = dto.Requested_Quantity,
                Status = "Pending",
                Request_Date = DateTime.UtcNow
            };

            await _customerRepository.AddPartRequestAsync(partRequest);
            await _customerRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Part request created for customer {CustomerId}.",
                customerId.Value
            );

            return "Part request submitted successfully. Please wait for staff approval.";
        }

        // Submits review for customer's completed appointment.
        public async Task<string> SubmitReviewAsync(
            CustomerReviewDto dto,
            string userId)
        {
            var customerId = await GetLoggedInCustomerIdAsync(userId);

            if (customerId == null)
                return "Customer profile not found.";

            if (dto.Rating < 1 || dto.Rating > 5)
                return "Rating must be between 1 and 5.";

            if (string.IsNullOrWhiteSpace(dto.Comment))
                return "Review comment is required.";

            var appointment = await _customerRepository
                .GetCustomerAppointmentAsync(
                    customerId.Value,
                    dto.Appointment_ID
                );

            if (appointment == null)
                return "Appointment not found for this customer.";

            if (appointment.Appointment_Status != "Completed")
                return "You can review only completed appointments.";

            var alreadyReviewed = await _customerRepository
                .ReviewExistsForAppointmentAsync(dto.Appointment_ID);

            if (alreadyReviewed)
                return "This appointment has already been reviewed.";

            var review = new Review
            {
                Customer_ID = customerId.Value,
                Appointment_ID = dto.Appointment_ID,
                Rating = dto.Rating,
                Comment = dto.Comment.Trim(),
                Review_Date = DateTime.UtcNow
            };

            await _customerRepository.AddReviewAsync(review);
            await _customerRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Review submitted for appointment {AppointmentId}.",
                dto.Appointment_ID
            );

            return "Review submitted successfully.";
        }

        // Gets vehicles owned by the logged-in customer.
        public async Task<List<CustomerVehicleListDto>> GetVehiclesAsync(string userId)
        {
            var customerId = await GetLoggedInCustomerIdAsync(userId);

            if (customerId == null)
                return new List<CustomerVehicleListDto>();

            var vehicles = await _customerRepository
                .GetCustomerVehiclesAsync(customerId.Value);

            return vehicles.Select(v => new CustomerVehicleListDto
            {
                Vehicle_ID = v.Vehicle_ID,
                Reg_Number = v.Reg_Number,
                Make = v.Make,
                Model = v.Model,
                Vehicle_Type = v.Vehicle_Type
            }).ToList();
        }

        // Gets all appointments of the logged-in customer.
        public async Task<List<CustomerAppointmentListDto>> GetAppointmentsAsync(string userId)
        {
            var customerId = await GetLoggedInCustomerIdAsync(userId);

            if (customerId == null)
                return new List<CustomerAppointmentListDto>();

            var appointments = await _customerRepository
                .GetCustomerAppointmentsAsync(customerId.Value);

            return appointments.Select(a => new CustomerAppointmentListDto
            {
                Appointment_ID = a.Appointment_ID,
                Vehicle_ID = a.Vehicle_ID,
                VehicleName = $"{a.Vehicle.Make} {a.Vehicle.Model} ({a.Vehicle.Reg_Number})",
                Appointment_Date = a.Appointment_Date,
                Service_Type = a.Service_Type,
                Appointment_Status = a.Appointment_Status
            }).ToList();
        }

        // Gets part requests created by the logged-in customer.
        public async Task<List<CustomerPartRequestListDto>> GetPartRequestsAsync(string userId)
        {
            var customerId = await GetLoggedInCustomerIdAsync(userId);

            if (customerId == null)
                return new List<CustomerPartRequestListDto>();

            var requests = await _customerRepository
                .GetCustomerPartRequestsAsync(customerId.Value);

            return requests.Select(r => new CustomerPartRequestListDto
            {
                Request_ID = r.Request_ID,
                Requested_Part_Name = r.Requested_Part_Name,
                Requested_Quantity = r.Requested_Quantity,
                Status = r.Status,
                Request_Date = r.Request_Date
            }).ToList();
        }

        // Gets completed appointments used in review dropdown.
        public async Task<List<CustomerAppointmentListDto>> GetCompletedAppointmentsAsync(string userId)
        {
            var customerId = await GetLoggedInCustomerIdAsync(userId);

            if (customerId == null)
                return new List<CustomerAppointmentListDto>();

            var appointments = await _customerRepository
                .GetCompletedCustomerAppointmentsAsync(customerId.Value);

            return appointments.Select(a => new CustomerAppointmentListDto
            {
                Appointment_ID = a.Appointment_ID,
                Vehicle_ID = a.Vehicle_ID,
                VehicleName = $"{a.Vehicle.Make} {a.Vehicle.Model} ({a.Vehicle.Reg_Number})",
                Appointment_Date = a.Appointment_Date,
                Service_Type = a.Service_Type,
                Appointment_Status = a.Appointment_Status
            }).ToList();
        }

        // Gets reviews submitted by the logged-in customer.
        public async Task<List<CustomerReviewListDto>> GetReviewsAsync(string userId)
        {
            var customerId = await GetLoggedInCustomerIdAsync(userId);

            if (customerId == null)
                return new List<CustomerReviewListDto>();

            var reviews = await _customerRepository
                .GetCustomerReviewsAsync(customerId.Value);

            return reviews.Select(r => new CustomerReviewListDto
            {
                Review_ID = r.Review_ID,
                Appointment_ID = r.Appointment_ID,
                Service_Type = r.Appointment.Service_Type,
                Rating = r.Rating,
                Comment = r.Comment,
                Review_Date = r.Review_Date
            }).ToList();
        }

        // Converts logged-in Identity user ID into Customer_ID.
        private async Task<int?> GetLoggedInCustomerIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            return await _customerRepository.GetCustomerIdByUserIdAsync(userId);
        }
    }
}