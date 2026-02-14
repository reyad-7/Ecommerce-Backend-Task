using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOS.Product
{
    public class ProductListDto
    {
        public List<ProductResponseDto> Products { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
