using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Extensions;
using Domain.DTOS.Product;
using Domain.Entities;
using Domain.Entities.Models;
using Domain.Interfaces.ICacheService;
using Domain.Interfaces.IProductService;
using Domain.Interfaces.IunitOfWork;
using static Domain.Entities.GeneralResponse.GeneralResponse;

namespace Application.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        
        private const string PRODUCT_LIST_CACHE_KEY = "products:list";
        private const string PRODUCT_DETAIL_CACHE_KEY = "products:detail";



        public ProductService(IUnitOfWork unitOfWork,ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<GeneralResponseDto<PaginatedResult<ProductResponseDto>>> GetAllProductsAsync(
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                // Validate parameters
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var cacheKey = $"{PRODUCT_LIST_CACHE_KEY}:page_{pageNumber}:size_{pageSize}";

                var cachedResult = await _cacheService.GetAsync<PaginatedResult<ProductResponseDto>>(cacheKey);
                if (cachedResult != null)
                {
                    return GeneralResponseDto<PaginatedResult<ProductResponseDto>>.SuccessResponse(
                        data: cachedResult,
                        message: $"Retrieved {cachedResult.Items.Count()} products from cache (Page {pageNumber} of {cachedResult.TotalPages})"
                    );
                }

                var pagedProducts = await _unitOfWork.Products.GetPagedAsync(
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    filter: null,
                    orderBy: q => q.OrderBy(p => p.Name),
                    includes: p => p.Category
                );

                var productDtos = pagedProducts.Items
                    .Select(p => p.ToResponseDto())
                    .ToList();

                var result = new PaginatedResult<ProductResponseDto>
                {
                    Items = productDtos,
                    TotalCount = pagedProducts.TotalCount,
                    PageNumber = pagedProducts.PageNumber,
                    PageSize = pagedProducts.PageSize,
                    TotalPages = pagedProducts.TotalPages
                };

                await _cacheService.SetAsync(cacheKey, result);

                return GeneralResponseDto<PaginatedResult<ProductResponseDto>>.SuccessResponse(
                    data: result,
                    message: $"Retrieved {productDtos.Count} products from database (Page {pageNumber} of {result.TotalPages})"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<PaginatedResult<ProductResponseDto>>.FailureResponse(
                    $"Error retrieving products: {ex.Message}"
                );
            }
        }


        public async Task<GeneralResponseDto<ProductResponseDto>> GetProductByIdAsync(int id)
        {
            try
            {
                var cacheKey = $"{PRODUCT_DETAIL_CACHE_KEY}:{id}";

                // try to get from cache first
                var cachedProduct = await _cacheService.GetAsync<ProductResponseDto>(cacheKey);
                if (cachedProduct != null)
                {
                    return GeneralResponseDto<ProductResponseDto>.SuccessResponse(
                        data: cachedProduct,
                        message: "Product retrieved from cache"
                    );
                }

                // If not in cache, get from database
                var product = await _unitOfWork.Products.FindAsync(
                    criteria: p => p.Id == id,
                    includes: new[] { "Category" }
                );

                if (product == null)
                {
                    return GeneralResponseDto<ProductResponseDto>.FailureResponse(
                        $"Product with ID {id} not found"
                    );
                }

                var productDto = product.ToResponseDto();

                //  Store in cache
                await _cacheService.SetAsync(cacheKey, productDto);

                return GeneralResponseDto<ProductResponseDto>.SuccessResponse(
                    data: productDto,
                    message: "Product retrieved from database"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<ProductResponseDto>.FailureResponse(
                    $"Error retrieving product: {ex.Message}"
                );
            }
        }

        public async Task<GeneralResponseDto<ProductResponseDto>> GetProductByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return GeneralResponseDto<ProductResponseDto>.FailureResponse(
                        "Product name cannot be empty"
                    );
                }

                var product = await _unitOfWork.Products.FindAsync(
                    criteria: p => p.Name == name,
                    includes: new[] { "Category" }
                );

                if (product is null)
                {
                    return GeneralResponseDto<ProductResponseDto>.FailureResponse(
                        $"Product with name '{name}' not found"
                    );
                }

                var productDto = product.ToResponseDto();

                return GeneralResponseDto<ProductResponseDto>.SuccessResponse(
                    data: productDto,
                    message: "Product retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<ProductResponseDto>.FailureResponse(
                    $"Error retrieving product: {ex.Message}"
                );
            }
        }
        public async Task<GeneralResponseDto<List<ProductResponseDto>>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                // First Verify category exists 
                var categoryExists = await _unitOfWork.Categories.GetByIdAsync(categoryId);
                if (categoryExists == null)
                {
                    return GeneralResponseDto<List<ProductResponseDto>>.FailureResponse(
                        $"Category with ID {categoryId} not found"
                    );
                }

                var products = await _unitOfWork.Products.FindAllAsync(
                    criteria: p => p.CategoryId == categoryId,
                    includes: new[] { "Category" }
                );

                var productDtos = products.Select(p => p.ToResponseDto()).ToList();

                return GeneralResponseDto<List<ProductResponseDto>>.SuccessResponse(
                    data: productDtos,
                    message: $"Retrieved {productDtos.Count} products for category '{categoryExists.Name}'"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<List<ProductResponseDto>>.FailureResponse(
                    $"Error retrieving products by category: {ex.Message}"
                );
            }
        }

        public async Task<GeneralResponseDto<List<ProductResponseDto>>> GetActiveProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.FindAllAsync(
                    criteria: p => p.IsActive,
                    includes: new[] { "Category" }
                );

                var productDtos = products.Select(p => p.ToResponseDto()).ToList();

                return GeneralResponseDto<List<ProductResponseDto>>.SuccessResponse(
                    data: productDtos,
                    message: $"Retrieved {productDtos.Count} active products"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<List<ProductResponseDto>>.FailureResponse(
                    $"Error retrieving active products: {ex.Message}"
                );
            }
        }

        public async Task<GeneralResponseDto<ProductResponseDto>> CreateProductAsync(CreateProductDto dto)
        {
            try
            {
                // Validate category if provided
                if (dto.CategoryId.HasValue)
                {
                    var categoryExists = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId.Value);
                    if (categoryExists == null)
                    {
                        return GeneralResponseDto<ProductResponseDto>.FailureResponse(
                            $"Category with ID {dto.CategoryId.Value} not found"
                        );
                    }
                }

                // Check if product name already exists
                var existingProduct = await _unitOfWork.Products.GetByNameAsync(dto.Name);
                if (existingProduct != null)
                {
                    return GeneralResponseDto<ProductResponseDto>.FailureResponse(
                        $"Product with name '{dto.Name}' already exists"
                    );
                }

                // Create new product
                var product = new Product
                {
                    Name = dto.Name.Trim(),
                    Description = dto.Description?.Trim(),
                    Price = dto.Price,
                    StockQuantity = dto.StockQuantity,
                    CategoryId = dto.CategoryId,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveAsync();

                var createdProduct = await _unitOfWork.Products.FindAsync(
                    criteria: p => p.Id == product.Id,
                    includes: new[] { "Category" }
                );

                var productDto = (createdProduct!.ToResponseDto());

                return GeneralResponseDto<ProductResponseDto>.SuccessResponse(
                    data: productDto,
                    message: "Product created successfully"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<ProductResponseDto>.FailureResponse(
                    $"Error creating product: {ex.Message}"
                );
            }
        }

        public async Task<GeneralResponseDto<ProductResponseDto>> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            try
            {
                // Check if product exists
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return GeneralResponseDto<ProductResponseDto>.FailureResponse(
                        $"Product with ID {id} not found"
                    );
                }

                // Validate category if provided
                if (dto.CategoryId.HasValue)
                {
                    var categoryExists = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId.Value);
                    if (categoryExists == null)
                    {
                        return GeneralResponseDto<ProductResponseDto>.FailureResponse(
                            $"Category with ID {dto.CategoryId.Value} not found"
                        );
                    }
                }

                // Check if new name already exists (excluding current product)
                if (dto.Name != product.Name)
                {
                    var nameExists = await _unitOfWork.Products.FindAsync(
                        p => p.Name == dto.Name && p.Id != id
                    );

                    if (nameExists != null)
                    {
                        return GeneralResponseDto<ProductResponseDto>.FailureResponse(
                            $"Product with name '{dto.Name}' already exists"
                        );
                    }
                }
                if (!string.IsNullOrWhiteSpace(dto.Name))
                {
                    product.Name = dto.Name.Trim();
                }

                if (dto.Description is not null)
                {
                    product.Description = dto.Description.Trim();
                }

                if (dto.Price.HasValue)
                {
                    product.Price = dto.Price.Value;
                }

                if (dto.StockQuantity.HasValue)
                {
                    product.StockQuantity = dto.StockQuantity.Value;
                }

                if (dto.IsActive.HasValue)
                {
                    product.IsActive = dto.IsActive.Value;
                }

                if (dto.CategoryId.HasValue)
                {
                    product.CategoryId = dto.CategoryId.Value;
                }

                product.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Products.Update(product);
                await _unitOfWork.SaveAsync();

                // Reload with category
                var updatedProduct = await _unitOfWork.Products.FindAsync(
                    criteria: p => p.Id == id,
                    includes: new[] { "Category" }
                );

                var productDto = updatedProduct!.ToResponseDto();

                return GeneralResponseDto<ProductResponseDto>.SuccessResponse(
                    data: productDto,
                    message: "Product updated successfully"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<ProductResponseDto>.FailureResponse(
                    $"Error updating product: {ex.Message}"
                );
            }
        }

        public async Task<GeneralResponseDto<bool>> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return GeneralResponseDto<bool>.FailureResponse(
                        $"Product with ID {id} not found"
                    );
                }

                // Soft delete: Set IsActive to false
                product.IsActive = false;
                product.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Products.Update(product);
                await _unitOfWork.SaveAsync();

                return GeneralResponseDto<bool>.SuccessResponse(
                    data: true,
                    message: "Product deactivated successfully"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<bool>.FailureResponse(
                    $"Error deleting product: {ex.Message}"
                );
            }
        }

        public async Task<GeneralResponseDto<bool>> HardDeleteProductAsync(int id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return GeneralResponseDto<bool>.FailureResponse(
                        $"Product with ID {id} not found"
                    );
                }

                // Check if product is in any orders
                var hasOrders = await _unitOfWork.OrderItems.FindAsync(oi => oi.ProductId == id);
                if (hasOrders != null)
                {
                    return GeneralResponseDto<bool>.FailureResponse(
                        "Cannot permanently delete product because it exists in order history"
                    );
                }

                // Hard delete
                _unitOfWork.Products.Delete(product);
                await _unitOfWork.SaveAsync();

                return GeneralResponseDto<bool>.SuccessResponse(
                    data: true,
                    message: "Product permanently deleted successfully"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<bool>.FailureResponse(
                    $"Error permanently deleting product: {ex.Message}"
                );
            }
        }
        public async Task<GeneralResponseDto<bool>> CheckStockAvailabilityAsync(int productId, int requiredQuantity)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                {
                    return GeneralResponseDto<bool>.FailureResponse(
                        $"Product with ID {productId} not found"
                    );
                }

                if (!product.IsActive)
                {
                    return GeneralResponseDto<bool>.FailureResponse(
                        "Product is not active"
                    );
                }

                bool isAvailable = product.StockQuantity >= requiredQuantity;

                return GeneralResponseDto<bool>.SuccessResponse(
                    data: isAvailable,
                    message: isAvailable
                        ? $"Stock available. Current stock: {product.StockQuantity}"
                        : $"Insufficient stock. Available: {product.StockQuantity}, Required: {requiredQuantity}"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<bool>.FailureResponse(
                    $"Error checking stock availability: {ex.Message}"
                );
            }
        }

        
    }
}
