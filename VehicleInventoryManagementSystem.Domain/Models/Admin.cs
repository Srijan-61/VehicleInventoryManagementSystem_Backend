using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VehicleInventoryManagementSystem.Domain.Models
{
    public class Admin
    {
        [Key]
        public int Admin_ID { get; set; }

        [ForeignKey(nameof(User))]
        public string User_Id { get; set; }

        public virtual User User { get; set; }
    }
}
