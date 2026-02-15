using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOS.Product
{
    public class ProductListDto
    {
<<<<<<< Updated upstream
        public List<ProductResponseDto> Products { get; set; } = new();
=======
        public List<ProductResponseDto> Products { get; set; } = new List<ProductResponseDto>();
>>>>>>> Stashed changes
        public int TotalCount { get; set; }
    }
}
