using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOS.Order
{
    public class OrderListDto
    {
        public List<OrderSummaryDto> Orders { get; set; } = new();
        public int TotalCount { get; set; }
    }
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public int ItemsCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
