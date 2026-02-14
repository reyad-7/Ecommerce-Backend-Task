using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOS.Category
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        [MinLength(2, ErrorMessage = "Category name must be at least 2 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }
}
