using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(string orderId, decimal amount);
    Task<bool> ConfirmPaymentAsync(string paymentIntentId);
}
