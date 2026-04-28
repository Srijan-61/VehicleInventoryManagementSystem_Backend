using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Text;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class User : IdentityUser<string>
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
    }
}
