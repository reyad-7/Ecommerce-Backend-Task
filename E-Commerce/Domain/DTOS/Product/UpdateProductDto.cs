using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOS.Product
{
    public class UpdateProductDto
    {
       
        [MaxLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
        [MinLength(2, ErrorMessage = "Product name must be at least 2 characters")]
        public string? Name { get; set; } 

        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }

        
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int? StockQuantity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
        public int? CategoryId { get; set; }
        public bool? IsActive { get; set; }
    }
}
