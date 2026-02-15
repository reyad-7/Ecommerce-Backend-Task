using System.Security.Claims;
using Domain.DTOS.Order;
using Domain.Entities;
using Domain.Interfaces.IOrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Domain.Entities.GeneralResponse.GeneralResponse;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Create a new order (authenticated users only)
        /// </summary>
        /// <param name="dto">Order creation data</param>
        /// <returns>Created order with details</returns>
        
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GeneralResponseDto<OrderResponseDto>.FailureResponse(
                    "Invalid input data. Please check your request."
                ));
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(GeneralResponseDto<OrderResponseDto>.FailureResponse(
                    "User authentication failed"
                ));
            }

            var response = await _orderService.CreateOrderAsync(userId, dto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get all orders for the authenticated user
        /// </summary>
        /// <returns>List of user's orders</returns>
        
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(GeneralResponseDto<PaginatedResult<OrderSummaryDto>>.FailureResponse(
                    "User authentication failed"
                ));
            }

            var response = await _orderService.GetUserOrdersAsync(userId, pageNumber, pageSize);

            if (!response.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get order by ID (user can only view their own orders)
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>Order details</returns>

        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(GeneralResponseDto<OrderResponseDto>.FailureResponse(
                    "User authentication failed"
                ));
            }

            var response = await _orderService.GetOrderByIdAsync(userId, orderId);

            if (!response.Success)
            {
                if (response.Message?.Contains("not authorized") == true)
                {
                    return Forbid();
                }
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get order by order number (user can only view their own orders)
        /// </summary>
        /// <param name="orderNumber">Order number (e.g., ORD-20240214120000-1234)</param>
        /// <returns>Order details</returns>
        
        [HttpGet("by-number/{orderNumber}")]
        public async Task<IActionResult> GetOrderByOrderNumber(string orderNumber)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(GeneralResponseDto<OrderResponseDto>.FailureResponse(
                    "User authentication failed"
                ));
            }

            var response = await _orderService.GetOrderByOrderNumberAsync(userId, orderNumber);

            if (!response.Success)
            {
                if (response.Message?.Contains("not authorized") == true)
                {
                    return Forbid();
                }
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Cancel an order (only pending orders can be cancelled)
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>Cancellation result</returns>
        
        [HttpPost("{orderId:int}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(GeneralResponseDto<bool>.FailureResponse(
                    "User authentication failed"
                ));
            }

            var response = await _orderService.CancelOrderAsync(userId, orderId);

            if (!response.Success)
            {
                if (response.Message?.Contains("not authorized") == true)
                {
                    return Forbid();
                }
                if (response.Message?.Contains("not found") == true)
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Update order status (Admin feature - can be used later)
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="dto">New status</param>
        /// <returns>Updated order</returns>
        
        [HttpPut("{orderId:int}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GeneralResponseDto<OrderResponseDto>.FailureResponse(
                    "Invalid input data. Please check your request."
                ));
            }

            var response = await _orderService.UpdateOrderStatusAsync(orderId, dto);

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

        // just get the current user ID from the JWT token claims
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}