using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IPaymentService _paymentService;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICartRepository cartRepository,
        IPaymentService paymentService)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _cartRepository = cartRepository;
        _paymentService = paymentService;
    }

    public async Task<List<OrderDto>> GetOrdersAsync(string userId, bool isAdmin)
    {
        var orders = isAdmin
            ? await _orderRepository.GetAllAsync()
            : await _orderRepository.GetByUserIdAsync(userId);

        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderDto> GetOrderByIdAsync(string orderId, string userId, bool isAdmin)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new Exception("Commande introuvable");

        if (order.UserId != userId && !isAdmin)
            throw new UnauthorizedAccessException("Accès non autorisé à cette commande");

        return MapToDto(order);
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto, string userId)
    {
        var order = new Order
        {
            UserId = userId,
            ShippingAddress = new Address
            {
                Street = dto.ShippingAddress.Street,
                City = dto.ShippingAddress.City,
                State = dto.ShippingAddress.State,
                ZipCode = dto.ShippingAddress.ZipCode,
                Country = dto.ShippingAddress.Country,
                IsDefault = dto.ShippingAddress.IsDefault
            }
        };

        decimal totalAmount = 0;

        foreach (var item in dto.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                throw new Exception($"Produit {item.ProductId} introuvable");
            }

            if (product.Stock < item.Quantity)
            {
                throw new Exception($"Stock insuffisant pour le produit {product.Name}");
            }

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = item.Quantity
            });

            totalAmount += product.Price * item.Quantity;

            // Update product stock
            product.Stock -= item.Quantity;
            await _productRepository.UpdateAsync(product);
        }

        order.TotalAmount = totalAmount;
        var createdOrder = await _orderRepository.CreateAsync(order);

        // Clear cart
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart != null)
        {
            cart.Items.Clear();
            await _cartRepository.UpdateAsync(cart);
        }

        return MapToDto(createdOrder);
    }

    public async Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(string orderId, string userId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new Exception("Commande introuvable");

        if (order.UserId != userId)
            throw new UnauthorizedAccessException("Accès non autorisé à cette commande");

        return await _paymentService.CreatePaymentIntentAsync(order.Id, order.TotalAmount);
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(string orderId, OrderStatus status)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new Exception("Commande introuvable");

        order.Status = status;

        // Si la commande est marquée comme livrée, enregistrer la date de livraison
        // pour démarrer le compte à rebours du droit de rétractation de 14 jours
        if (status == OrderStatus.Delivered && !order.DeliveredAt.HasValue)
        {
            order.DeliveredAt = DateTime.UtcNow;
        }

        await _orderRepository.UpdateAsync(order);

        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto(
            order.Id,
            order.UserId,
            order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.Quantity, i.Price)).ToList(),
            order.TotalAmount,
            order.Status,
            new AddressDto(
                order.ShippingAddress.Street,
                order.ShippingAddress.City,
                order.ShippingAddress.State,
                order.ShippingAddress.ZipCode,
                order.ShippingAddress.Country,
                order.ShippingAddress.IsDefault
            ),
            order.PaymentStatus,
            order.CreatedAt,
            order.DeliveredAt,
            order.ReturnRequestedAt,
            order.ReturnReason,
            order.ReturnStatus,
            order.ReturnDeadline,
            order.CanReturn,
            order.EstimatedDeliveryDate,
            order.ShippedAt,
            order.TrackingNumber,
            order.CarrierName,
            order.EstimatedDeliveryDays,
            order.IsDeliveryDelayed
        );
    }
}
