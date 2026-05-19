using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using VehicleInventoryManagementSystem.Application.Wrappers;

namespace VehicleInventoryManagementSystem.API.Middlewares
{
    
    /// Handles unexpected errors globally across    the application.
    /// If any request throws an unhandled exception,
    /// this middleware catches it and returns a clean ApiResponse JSON object.
    /// Helps avoid exposing internal server errors or stack traces to the frontend.
  
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Let the request proceed normally through the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // If any unhandled exception occurs, catch it here and return a safe JSON response
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // Create the standard error model using our ApiResponse wrapper
                var responseModel = new ApiResponse<string>(
                    message: "An unexpected internal server error occurred.",
                    errors: new List<string> { ex.Message } // In production, hide ex.Message for security
                );

                var result = JsonSerializer.Serialize(responseModel, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await response.WriteAsync(result);
            }
        }
    }
}
