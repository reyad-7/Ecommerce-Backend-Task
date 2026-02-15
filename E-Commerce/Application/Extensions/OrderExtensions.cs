using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOS.Order;
using Domain.Entities.Models;

namespace Application.Extensions
{
    public static class OrderExtensions
    {
        public static OrderResponseDto ToResponseDto(this Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User?.UserName,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                OrderNumber = order.OrderNumber,
                OrderStatusName = order.OrderStatus.ToString(),
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice,
                    Id = oi.Id
                }).ToList()
            };
        }
    }
}
