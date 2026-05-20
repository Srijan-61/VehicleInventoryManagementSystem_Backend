using System;
using System.Collections.Generic;

namespace VehicleInventoryManagementSystem.Application.DTOs
{
    public class CustomerHistoryItemDto
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public decimal? TotalAmount { get; set; }
        public string Status { get; set; }
    }

    public class CustomerHistoryResultDto
    {
        public List<CustomerHistoryItemDto> HistoryItems { get; set; } = new List<CustomerHistoryItemDto>();
    }
}
