using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class Role : IdentityRole<string>
    {
        public string Description { get; set; }
    }
}
