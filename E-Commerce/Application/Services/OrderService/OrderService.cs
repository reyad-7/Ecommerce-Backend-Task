using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Extensions;
using Domain.DTOS.Order;
using Domain.Entities.Enums;
using Domain.Entities.GeneralResponse;
using Domain.Entities.Models;
using Domain.Interfaces.IOrderService;
using Domain.Interfaces.IunitOfWork;
using static Domain.Entities.GeneralResponse.GeneralResponse;

namespace Application.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralResponse.GeneralResponseDto<OrderResponseDto>> CreateOrderAsync(string userId, CreateOrderDto dto)
        {
            try { 
            var user = await _unitOfWork.Users.FindAsync(u=>u.Id == userId);
            if (user is null)
            {
                return GeneralResponse.GeneralResponseDto<OrderResponseDto>.FailureResponse("User not found.");
            }
            if (dto.OrderItems == null || !dto.OrderItems.Any())
            {
                return GeneralResponseDto<OrderResponseDto>.FailureResponse(
                    "Order must contain at least one item"
                );
            }
            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var item in dto.OrderItems)
            {
                var product = await _unitOfWork.Products.FindAsync(p => p.Id == item.ProductId);
                if (product == null)
                {
                    return GeneralResponseDto<OrderResponseDto>.FailureResponse(
                        $"Product with ID {item.ProductId} not found"
                    );
                }

                if (product.StockQuantity < item.Quantity)
                {
                    return GeneralResponseDto<OrderResponseDto>.FailureResponse(
                        $"Insufficient stock for product {product.Name}. Available: {product.StockQuantity}, Requested: {item.Quantity}"
                    );
                }
                if (!product.IsActive)
                {
                    return GeneralResponseDto<OrderResponseDto>.FailureResponse(
                        $"Product {product.Name} is not available for purchase"
                    );
                }

                if (item.Quantity <= 0)
                {
                    return GeneralResponseDto<OrderResponseDto>.FailureResponse(
                        $"Quantity for product {product.Name} must be greater than zero"
                    );
                }

                // Calculate item total
                var itemTotal = product.Price * item.Quantity;
                totalAmount += itemTotal;

                // Create order item
                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name, 
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = itemTotal
                };
                orderItems.Add( orderItem );
            }
            var orderNumber = await GenerateOrderNumberAsync();
            var order = new Order
            {
                UserId = userId,
                OrderNumber = orderNumber,
                TotalAmount = totalAmount,
                OrderStatus = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };
            await _unitOfWork.Orders.AddAsync(order);
            // update product stock
            foreach (var itemDto in dto.OrderItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= itemDto.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Products.Update(product);
                }
            }
            await _unitOfWork.SaveAsync();

            var createdOrder = await _unitOfWork.Orders.FindAsync(
                   criteria: o => o.Id == order.Id,
                   includes: new[] { "OrderItems", "User" }
               );

                var orderResponse = createdOrder.ToResponseDto();
                return GeneralResponseDto<OrderResponseDto>.SuccessResponse(orderResponse, "Order created successfully.");
            }

            catch (Exception ex)
            {
                return GeneralResponseDto<OrderResponseDto>.FailureResponse("An error occurred while creating the order.");
            }

        }
        public async Task<GeneralResponse.GeneralResponseDto<OrderResponseDto>> GetOrderByIdAsync(string userId, int orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.FindAsync(
                    criteria: o => o.Id == orderId && o.UserId == userId,
                    includes: new[] { "OrderItems", "User", "OrderItems.Product" }
                );
                if (order == null)
                {
                    return GeneralResponse.GeneralResponseDto<OrderResponseDto>.FailureResponse("Order not found.");
                }
                if (order.UserId != userId)
                {
                    return GeneralResponseDto<OrderResponseDto>.FailureResponse(
                        "You are not authorized to view this order"
                    );
                }
                var orderResponse = order.ToResponseDto();
                return GeneralResponseDto<OrderResponseDto>.SuccessResponse(
                    data: orderResponse,
                    message: "Order retrieved successfully"
                );

            }
            catch (Exception ex)
            {
                return GeneralResponse.GeneralResponseDto<OrderResponseDto>.FailureResponse("An error occurred while retrieving the order.");
            }
        }

        public async Task<GeneralResponse.GeneralResponseDto<OrderListDto>> GetUserOrdersAsync(string userId)
        {
            try { 
            var orders = await _unitOfWork.Orders.FindAllAsync(
                   criteria: o => o.UserId == userId,
                   includes: new[] { "OrderItems" }
               );

            var orderSummaries = orders.Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                TotalAmount = o.TotalAmount,
                OrderStatus = o.OrderStatus.ToString(),
                ItemsCount = o.OrderItems?.Count ?? 0,
                CreatedAt = o.CreatedAt
            }).OrderByDescending(o => o.CreatedAt).ToList();

            var ordersListDto = new OrderListDto
            {
                Orders = orderSummaries,
                TotalCount = orderSummaries.Count
            };
                return GeneralResponseDto<OrderListDto>.SuccessResponse(
                         data: ordersListDto,
                         message: $"Retrieved {orderSummaries.Count} orders"
                     );
            }
            catch (Exception ex)
            {
                return GeneralResponse.GeneralResponseDto<OrderListDto>.FailureResponse("An error occurred while retrieving user orders.");
            }
        }

        public async Task<GeneralResponse.GeneralResponseDto<OrderResponseDto>> GetOrderByOrderNumberAsync(string userId, string orderNumber)
        {
            try
            {
                var order = await _unitOfWork.Orders.FindAsync(
                        criteria: o => o.OrderNumber == orderNumber,
                        includes: new[] { "OrderItems", "User" }
                    );

                if (order == null)
                {
                    return GeneralResponse.GeneralResponseDto<OrderResponseDto>.FailureResponse("Order not found.");
                }
                // ownership check
                if (order.UserId != userId)
                {
                    return GeneralResponseDto<OrderResponseDto>.FailureResponse(
                        "You are not authorized to view this order"
                    );
                }
                var orderResponse = order.ToResponseDto();
                return GeneralResponseDto<OrderResponseDto>.SuccessResponse(
                    data: orderResponse,
                    message: "Order retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponse.GeneralResponseDto<OrderResponseDto>.FailureResponse("An error occurred while retrieving the order.");

            }
        }
        
        public async Task<GeneralResponse.GeneralResponseDto<OrderResponseDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            try
            {
                var order = await  _unitOfWork.Orders.FindAsync(
                        criteria: o => o.Id == orderId,
                        includes: new[] { "OrderItems", "User", "OrderItems.Product" }
                    );
                if (order == null)
                {
                    return GeneralResponse.GeneralResponseDto<OrderResponseDto>.FailureResponse("Order not found.");
                }
                order.OrderStatus = dto.OrderStatus;
                order.UpdatedAt = DateTime.UtcNow;
                var orderResponse = order.ToResponseDto();

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveAsync();

                return GeneralResponseDto<OrderResponseDto>.SuccessResponse(
                    data: orderResponse,
                    message: $"Order status updated to {dto.OrderStatus}"
                );
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<OrderResponseDto>.FailureResponse(
                    $"Error updating order status: {ex.Message}"
                );
            }
        }


        public async Task<GeneralResponse.GeneralResponseDto<bool>> CancelOrderAsync(string userId, int orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.FindAsync(
                    criteria: o => o.Id == orderId,
                    includes: new[] { "OrderItems" }
                );

                if (order == null)
                {
                    return GeneralResponseDto<bool>.FailureResponse(
                        $"Order with ID {orderId} not found"
                    );
                }
                if (order.UserId != userId)
                {
                    return GeneralResponseDto<bool>.FailureResponse(
                        "You are not authorized to cancel this order"
                    );
                }
                if (order.OrderStatus == OrderStatus.Cancelled)
                {
                    return GeneralResponseDto<bool>.FailureResponse(
                        "Order is already canceled"
                    );
                }
                // Only allow cancellation if order is still pending
                if (order.OrderStatus != OrderStatus.Pending)
                {
                    return GeneralResponseDto<bool>.FailureResponse(
                        $"Cannot cancel order with status '{order.OrderStatus}'. Only pending orders can be cancelled."
                    );
                }

                order.OrderStatus = OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Orders.Update(order);
                // Restock products
                foreach (var item in order.OrderItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                        product.UpdatedAt = DateTime.UtcNow;
                        _unitOfWork.Products.Update(product);
                    }
                }
                await _unitOfWork.SaveAsync();
                return GeneralResponseDto<bool>.SuccessResponse(
                   data: true,
                   message: "Order cancelled successfully. Also Stock has been restored."
               );
            }
            catch (Exception ex)
            {
                return GeneralResponse.GeneralResponseDto<bool>.FailureResponse("An error occurred while canceling the order.");
            }

            }

        private async Task<string> GenerateOrderNumberAsync()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            var orderNumber = $"ORD-{timestamp}-{random}";

            // Ensure uniqueness
            var exists = await _unitOfWork.Orders.FindAsync(o => o.OrderNumber == orderNumber);
            if (exists != null)
            {
                // Recursive call if collision (very unlikely)
                return await GenerateOrderNumberAsync();
            }

            return orderNumber;
        }
    }
}
