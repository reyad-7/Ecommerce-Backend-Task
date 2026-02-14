using Domain.DTOS.Category;
using Domain.Interfaces;
using Domain.Interfaces.ICategoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Domain.Entities.GeneralResponse.GeneralResponse;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Get all categories with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>Paginated list of categories</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _categoryService.GetAllCategoriesAsync(pageNumber, pageSize);

            if (!response.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(GeneralResponseDto<CategoryResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResponseDto<CategoryResponseDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var response = await _categoryService.GetCategoryByIdAsync(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get category by name
        /// </summary>
        /// <param name="name">Category name</param>
        /// <returns>Category details</returns>
        
        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetCategoryByName(string name)
        {
            var response = await _categoryService.GetCategoryByNameAsync(name);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get all categories with their associated products
        /// </summary>
        /// <returns>List of categories with product details</returns>
        
        [HttpGet("with-products")]
        public async Task<IActionResult> GetCategoriesWithProducts()
        {
            var response = await _categoryService.GetCategoriesWithProductsAsync();

            if (!response.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        /// <param name="dto">Category creation data</param>
        /// <returns>Created category</returns>
       
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GeneralResponseDto<CategoryResponseDto>.FailureResponse(
                    "Invalid input data. Please check your request."
                ));
            }

            var response = await _categoryService.CreateCategoryAsync(dto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Update an existing category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="dto">Category update data</param>
        /// <returns>Updated category</returns>
        [HttpPut("{id:int}")]
       public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GeneralResponseDto<CategoryResponseDto>.FailureResponse(
                    "Invalid input data. Please check your request."
                ));
            }

            var response = await _categoryService.UpdateCategoryAsync(id, dto);

            if (!response.Success)
            {
                if (response.Message?.Contains("not found") == true)
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Deletion result</returns>
        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _categoryService.DeleteCategoryAsync(id);

            if (!response.Success)
            {
                if (response.Message?.Contains("not found") == true)
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response);
        }

    }
}