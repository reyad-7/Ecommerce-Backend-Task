using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOS.Order;
using Domain.Entities;
using static Domain.Entities.GeneralResponse.GeneralResponse;

namespace Domain.Interfaces.IOrderService
{
    public interface IOrderService
    {
        // Create new order (authenticated user)
        Task<GeneralResponseDto<OrderResponseDto>> CreateOrderAsync(string userId, CreateOrderDto dto);

        // Get order by ID (with ownership check)
        Task<GeneralResponseDto<OrderResponseDto>> GetOrderByIdAsync(string userId, int orderId);

        // Get all orders for a specific user
        Task<GeneralResponseDto<PaginatedResult<OrderSummaryDto>>> GetUserOrdersAsync(
            string userId,
            int pageNumber = 1,
            int pageSize = 10);

        // Get order by order number (with ownership check)
        Task<GeneralResponseDto<OrderResponseDto>> GetOrderByOrderNumberAsync(string userId, string orderNumber);

        // Update order status (admin feature - optional for now)
        Task<GeneralResponseDto<OrderResponseDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);

        // Cancel order (user can cancel their own order if status is Pending)
        Task<GeneralResponseDto<bool>> CancelOrderAsync(string userId, int orderId);
    }

}
