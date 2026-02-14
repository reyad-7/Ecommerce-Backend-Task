using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOS.Category;
using static Domain.Entities.GeneralResponse.GeneralResponse;

namespace Domain.Interfaces.ICategoryService
{
    public interface ICategoryService
    {
        // Get all categories (with optional pagination)
        Task<GeneralResponseDto<Domain.DTOS.Category.CategoryListDto>> GetAllCategoriesAsync(int pageNumber = 1, int pageSize = 10);

        // Get category by ID
        Task<GeneralResponseDto<CategoryResponseDto>> GetCategoryByIdAsync(int id);

        // Get category by name
        Task<GeneralResponseDto<CategoryResponseDto>> GetCategoryByNameAsync(string name);

        // Create new category
        Task<GeneralResponseDto<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto dto);

        // Update existing category
        Task<GeneralResponseDto<CategoryResponseDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto);

        // Delete category
        Task<GeneralResponseDto<bool>> DeleteCategoryAsync(int id);

        // Get categories with their products
        Task<GeneralResponseDto<List<CategoryWithProductsDto>>> GetCategoriesWithProductsAsync();

    }
}
