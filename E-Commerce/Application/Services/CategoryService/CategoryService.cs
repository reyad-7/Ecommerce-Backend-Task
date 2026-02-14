using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOS.Category;
using Domain.Entities.GeneralResponse;
using Domain.Entities.Models;
using Domain.Interfaces.ICategoryService;
using Domain.Interfaces.IunitOfWork;
using static Domain.Entities.GeneralResponse.GeneralResponse;


namespace Application.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralResponse.GeneralResponseDto<Domain.DTOS.Category.CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto dto)
        {
            try { 
            var existingCategory =await _unitOfWork.Categories.GetByNameAsync(dto.Name);
            if (existingCategory != null)
            {
                return GeneralResponse.GeneralResponseDto<Domain.DTOS.Category.CategoryResponseDto>.FailureResponse("Category with the same name already exists.");
            }

            // Create new category entity
            var category = new Category
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveAsync();
            var categoryResponse = new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
            return GeneralResponse.GeneralResponseDto<Domain.DTOS.Category. CategoryResponseDto>.SuccessResponse(categoryResponse, "Category created successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.GeneralResponseDto<Domain.DTOS.Category.CategoryResponseDto>.FailureResponse($"Error creating category: {ex.Message}");
            }
        }

        public async Task<GeneralResponse.GeneralResponseDto<bool>> DeleteCategoryAsync(int id)
        {
            try { 
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                return GeneralResponse.GeneralResponseDto<bool>.FailureResponse($"Category With id: {id} not found.");
            }
            var categoryWithProducts = await _unitOfWork.Categories.FindAsync(
                    c => c.Id == id,
                    includes: new[] { "Products" }
                );

            if (categoryWithProducts?.Products != null && categoryWithProducts.Products.Any())
            {
                return GeneralResponseDto<bool>.FailureResponse(
                    $"Cannot delete category '{category.Name}' because it has {categoryWithProducts.Products.Count} associated products"
                );
            }
            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveAsync();
            return GeneralResponse.GeneralResponseDto<bool>.SuccessResponse(true, $"Category {category.Name} deleted successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<bool>.FailureResponse(
                    $"Error deleting category: {ex.Message}"
                );
            }

        }

       

        public async Task<GeneralResponse.GeneralResponseDto<List<Domain.DTOS.Category.CategoryWithProductsDto>>> GetCategoriesWithProductsAsync()
        {
            try
            {
                var categories = await _unitOfWork.Categories.FindAllAsync(
                        criteria: c => true,
                        includes: new[] { "Products" }
                    );

                var categoryWithProductsDtos = categories.Select(c => new CategoryWithProductsDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ProductCount = c.Products?.Count ?? 0,
                    Products = c.Products?.Select(p => new ProductSummaryDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        StockQuantity = p.StockQuantity,
                        IsActive = p.IsActive
                    }).ToList() ?? new List<ProductSummaryDto>()
                }).ToList();

                return GeneralResponseDto<List<CategoryWithProductsDto>>.SuccessResponse(
                   data: categoryWithProductsDtos,
                   message: $"Retrieved {categoryWithProductsDtos.Count} categories with products"
               );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<List<CategoryWithProductsDto>>.FailureResponse(
                   $"Error retrieving categories with products: {ex.Message}"
               );
            }
        }

        public async Task<GeneralResponse.GeneralResponseDto<Domain.DTOS.Category.CategoryResponseDto>> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                {
                    return GeneralResponseDto<CategoryResponseDto>.FailureResponse(
                        $"Category with ID {id} not found"
                    );
                }
                var categoryDto = new CategoryResponseDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                };

                return GeneralResponseDto<CategoryResponseDto>.SuccessResponse(
                    data: categoryDto,
                    message: "Category retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<CategoryResponseDto>.FailureResponse(
                   $"Error retrieving category: {ex.Message}"
               );
            }
        }

        public async Task<GeneralResponse.GeneralResponseDto<Domain.DTOS.Category.CategoryResponseDto>> GetCategoryByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return GeneralResponseDto<CategoryResponseDto>.FailureResponse(
                        "Category name cannot be empty"
                    );
                }
                var category = await _unitOfWork.Categories.GetByNameAsync(name);

                if (category == null)
                {
                    return GeneralResponseDto<CategoryResponseDto>.FailureResponse(
                        $"Category with name '{name}' not found"
                    );
                }

                var categoryDto = new CategoryResponseDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                };
                return GeneralResponseDto<CategoryResponseDto>.SuccessResponse(
                    data: categoryDto,
                    message: "Category retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<CategoryResponseDto>.FailureResponse(
                    $"Error retrieving category: {ex.Message}"
                );
            }
        }

        public async Task<GeneralResponse.GeneralResponseDto<Domain.DTOS.Category.CategoryResponseDto>> UpdateCategoryAsync(int id,UpdateCategoryDto dto)
        {
            try
            {
                // Check if category exists
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                {
                    return GeneralResponseDto<CategoryResponseDto>.FailureResponse(
                        $"Category with ID {id} not found"
                    );
                }

                // Check if new name already exists (excluding current category)
                if (dto.Name != category.Name)
                {
                    var nameExists = await _unitOfWork.Categories.FindAsync(
                        c => c.Name == dto.Name && c.Id != id
                    );

                    if (nameExists != null)
                    {
                        return GeneralResponseDto<CategoryResponseDto>.FailureResponse(
                            $"Category with name '{dto.Name}' already exists"
                        );
                    }
                }

                category.Name = dto.Name.Trim();
                category.Description = dto.Description?.Trim();
                category.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.SaveAsync();

                var categoryDto = new CategoryResponseDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                };

                return GeneralResponseDto<CategoryResponseDto>.SuccessResponse(
                    data: categoryDto,
                    message: "Category updated successfully"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<CategoryResponseDto>.FailureResponse(
                    $"Error updating category: {ex.Message}"
                );
            }
        }

        async Task<GeneralResponseDto<CategoryListDto>>ICategoryService.GetAllCategoriesAsync(int pageNumber, int pageSize)
        {
            try
            {
                // Validate pagination parameters
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                // Get total count
                var totalCount = await _unitOfWork.Categories.CountAsync();

                // Calculate skip
                var skip = (pageNumber - 1) * pageSize;

                // Get paginated categories
                var categories = await _unitOfWork.Categories.GetAllAsync();

                // Map to DTOs
                var categoryDtos = categories.Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();

                var result = new CategoryListDto
                {
                    Categories = categoryDtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return GeneralResponseDto<CategoryListDto>.SuccessResponse(
                    data: result,
                    message: $"Retrieved {categoryDtos.Count} categories successfully"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<CategoryListDto>.FailureResponse(
                    $"Error retrieving categories: {ex.Message}"
                );
            }
        }
    }
}
