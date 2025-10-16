using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces;

public interface IOrderService
{
    Task<List<OrderDto>> GetOrdersAsync(string userId, bool isAdmin);
    Task<OrderDto> GetOrderByIdAsync(string orderId, string userId, bool isAdmin);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto dto, string userId);
    Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(string orderId, string userId);
    Task<OrderDto> UpdateOrderStatusAsync(string orderId, OrderStatus status);
}
