using Domain.DTOS.Product;
using Domain.Interfaces.IProductService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Domain.Entities.GeneralResponse.GeneralResponse;

namespace E_Commerce.Controllers.ProductController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>List of all products</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllProducts(
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10)
        {
            var response = await _productService.GetAllProductsAsync(pageNumber, pageSize);

            if (!response.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var response = await _productService.GetProductByIdAsync(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get product by name
        /// </summary>
        /// <param name="name">Product name</param>
        /// <returns>Product details</returns>
        
        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetProductByName(string name)
        {
            var response = await _productService.GetProductByNameAsync(name);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get all products by category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of products in the category</returns>
        
        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var response = await _productService.GetProductsByCategoryAsync(categoryId);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get all active products only
        /// </summary>
        /// <returns>List of active products</returns>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveProducts()
        {
            var response = await _productService.GetActiveProductsAsync();

            if (!response.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="dto">Product creation data</param>
        /// <returns>Created product</returns>
        
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GeneralResponseDto<ProductResponseDto>.FailureResponse(
                    "Invalid input data. Please check your request."
                ));
            }

            var response = await _productService.CreateProductAsync(dto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="dto">Product update data</param>
        /// <returns>Updated product</returns>
        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GeneralResponseDto<ProductResponseDto>.FailureResponse(
                    "Invalid input data. Please check your request."
                ));
            }

            var response = await _productService.UpdateProductAsync(id, dto);

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
        /// Soft delete a product (deactivate)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Deletion result</returns>
        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProductAsync(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Permanently delete a product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Deletion result</returns>
        
        [HttpDelete("{id:int}/permanent")]
        public async Task<IActionResult> HardDeleteProduct(int id)
        {
            var response = await _productService.HardDeleteProductAsync(id);

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
        /// Check if sufficient stock is available for a product
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="quantity">Required quantity</param>
        /// <returns>Stock availability status</returns>
        
        [HttpGet("{productId:int}/check-stock")]
        public async Task<IActionResult> CheckStockAvailability(int productId, [FromQuery] int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest(GeneralResponseDto<bool>.FailureResponse(
                    "Quantity must be greater than 0"
                ));
            }

            var response = await _productService.CheckStockAvailabilityAsync(productId, quantity);

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
