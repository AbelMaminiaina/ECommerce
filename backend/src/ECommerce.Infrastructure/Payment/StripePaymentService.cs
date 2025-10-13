using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.Infrastructure.Payment;

public class StripePaymentService : IPaymentService
{
    private readonly IOrderRepository _orderRepository;
    private readonly string _secretKey;

    public StripePaymentService(IOrderRepository orderRepository, IOptions<StripeSettings> stripeSettings)
    {
        _orderRepository = orderRepository;
        _secretKey = stripeSettings.Value.SecretKey;
        StripeConfiguration.ApiKey = _secretKey;
    }

    public async Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(string orderId, decimal amount)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new Exception("Order not found");
        }

        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100), // Convert to cents
            Currency = "usd",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            },
            Metadata = new Dictionary<string, string>
            {
                { "orderId", orderId }
            }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        order.PaymentIntentId = paymentIntent.Id;
        await _orderRepository.UpdateAsync(order);

        return new PaymentIntentResponseDto(paymentIntent.ClientSecret, paymentIntent.Id);
    }

    public async Task<bool> ConfirmPaymentAsync(string paymentIntentId)
    {
        var service = new PaymentIntentService();
        var paymentIntent = await service.GetAsync(paymentIntentId);

        var order = await _orderRepository.GetByPaymentIntentIdAsync(paymentIntentId);
        if (order == null)
        {
            return false;
        }

        if (paymentIntent.Status == "succeeded")
        {
            order.PaymentStatus = Domain.Entities.PaymentStatus.Completed;
            order.Status = Domain.Entities.OrderStatus.Processing;
            await _orderRepository.UpdateAsync(order);
            return true;
        }

        return false;
    }
}
