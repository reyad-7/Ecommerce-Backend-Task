using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Enums;

namespace Domain.DTOS.Order
{
    public class UpdateOrderStatusDto
    {
        [Required(ErrorMessage = "Order status is required")]
        public OrderStatus OrderStatus { get; set; }
    }
}
