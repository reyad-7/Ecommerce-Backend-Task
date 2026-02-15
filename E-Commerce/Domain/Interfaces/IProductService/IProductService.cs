using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOS.Product;
using Domain.Entities;
using static Domain.Entities.GeneralResponse.GeneralResponse;

namespace Domain.Interfaces.IProductService
{
    public interface IProductService
    {
        Task<GeneralResponseDto< PaginatedResult<ProductListDto>>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 10);
        Task<GeneralResponseDto<ProductResponseDto>> GetProductByIdAsync(int id);
        Task<GeneralResponseDto<ProductResponseDto>> GetProductByNameAsync(string name);
        Task<GeneralResponseDto<List<ProductResponseDto>>> GetProductsByCategoryAsync(int categoryId);

        Task<GeneralResponseDto<List<ProductResponseDto>>> GetActiveProductsAsync();

        // Get products with low stock (for inventory alerts)
        // Task<GeneralResponseDto<List<ProductResponseDto>>> GetLowStockProductsAsync(int threshold = 10);

        // Create new product
        Task<GeneralResponseDto<ProductResponseDto>> CreateProductAsync(CreateProductDto dto);

        // Update existing product
        Task<GeneralResponseDto<ProductResponseDto>> UpdateProductAsync(int id, UpdateProductDto dto);

        // Delete product (soft delete by setting IsActive = false)
        Task<GeneralResponseDto<bool>> DeleteProductAsync(int id);

        // Hard delete product (permanent deletion)
        Task<GeneralResponseDto<bool>> HardDeleteProductAsync(int id);

        // Update stock quantity
        // Task<GeneralResponseDto<ProductResponseDto>> UpdateStockAsync(int id, UpdateStockDto dto);

        // Check if sufficient stock is available
        Task<GeneralResponseDto<bool>> CheckStockAvailabilityAsync(int productId, int requiredQuantity);

        // Activate/Deactivate product
        // Task<GeneralResponseDto<ProductResponseDto>> ToggleProductStatusAsync(int id);
    }
}
