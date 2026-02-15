using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOS.Order
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Order items are required")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<CreateOrderItemDto> OrderItems { get; set; } = new();
    }
    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
