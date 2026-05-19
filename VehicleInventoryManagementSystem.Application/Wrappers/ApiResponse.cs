using System.Collections.Generic;

namespace VehicleInventoryManagementSystem.Application.Wrappers
{
    
    /// Common API response wrapper used across the application.
    /// Keeps responses consistent by always returning:
    /// Success status, message, data payload, and validationerrors.
   
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        // Success response constructor
        public ApiResponse(T data, string message = null)
        {
            Success = true;
            Message = message;
            Data = data;
            Errors = null;
        }

        // Error response constructor
        public ApiResponse(string message, List<string> errors = null)
        {
            Success = false;
            Message = message;
            Data = default;
            Errors = errors;
        }
    }
}
