using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services;

public class ShippingService : IShippingService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IShippingMethodRepository _shippingMethodRepository;

    public ShippingService(
        IOrderRepository orderRepository,
        IShippingMethodRepository shippingMethodRepository)
    {
        _orderRepository = orderRepository;
        _shippingMethodRepository = shippingMethodRepository;
    }

    public async Task<List<ShippingMethodDto>> GetAllShippingMethodsAsync()
    {
        var methods = await _shippingMethodRepository.GetActiveMethodsAsync();
        return methods.Select(MapToDto).ToList();
    }

    public async Task<ShippingMethodDto> GetShippingMethodByIdAsync(string id)
    {
        var method = await _shippingMethodRepository.GetByIdAsync(id);
        if (method == null)
            throw new Exception("Méthode de livraison introuvable");

        return MapToDto(method);
    }

    public async Task<ShippingMethodDto> CreateShippingMethodAsync(CreateShippingMethodDto dto)
    {
        var method = new ShippingMethod
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            MinDeliveryDays = dto.MinDeliveryDays,
            MaxDeliveryDays = dto.MaxDeliveryDays,
            CarrierName = dto.CarrierName,
            IsActive = true
        };

        var created = await _shippingMethodRepository.CreateAsync(method);
        return MapToDto(created);
    }

    public async Task<ShippingMethodDto> UpdateShippingMethodAsync(string id, UpdateShippingMethodDto dto)
    {
        var method = await _shippingMethodRepository.GetByIdAsync(id);
        if (method == null)
            throw new Exception("Méthode de livraison introuvable");

        if (dto.Name != null) method.Name = dto.Name;
        if (dto.Description != null) method.Description = dto.Description;
        if (dto.Price.HasValue) method.Price = dto.Price.Value;
        if (dto.MinDeliveryDays.HasValue) method.MinDeliveryDays = dto.MinDeliveryDays.Value;
        if (dto.MaxDeliveryDays.HasValue) method.MaxDeliveryDays = dto.MaxDeliveryDays.Value;
        if (dto.CarrierName != null) method.CarrierName = dto.CarrierName;
        if (dto.IsActive.HasValue) method.IsActive = dto.IsActive.Value;

        await _shippingMethodRepository.UpdateAsync(method);
        return MapToDto(method);
    }

    public async Task DeleteShippingMethodAsync(string id)
    {
        await _shippingMethodRepository.DeleteAsync(id);
    }

    public async Task<List<ShippingMethodDto>> GetAvailableShippingMethodsAsync(string orderId, string userId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new Exception("Commande introuvable");

        if (order.UserId != userId)
            throw new UnauthorizedAccessException("Accès non autorisé");

        var methods = await _shippingMethodRepository.GetActiveMethodsAsync();
        return methods.Select(MapToDto).ToList();
    }

    public async Task<TrackingInfoDto> GetTrackingAsync(string orderId, string userId, bool isAdmin)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new Exception("Commande introuvable");

        // Vérifier que la commande appartient à l'utilisateur (sauf admin)
        if (order.UserId != userId && !isAdmin)
            throw new UnauthorizedAccessException("Accès non autorisé");

        return new TrackingInfoDto(
            order.TrackingNumber,
            order.CarrierName,
            order.ShippedAt,
            order.EstimatedDeliveryDate,
            order.DeliveredAt,
            order.IsDeliveryDelayed
        );
    }

    public async Task<TrackingInfoDto> UpdateShippingInfoAsync(string orderId, UpdateShippingDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new Exception("Commande introuvable");

        // Mettre à jour les informations d'expédition
        order.TrackingNumber = dto.TrackingNumber;
        order.CarrierName = dto.CarrierName;
        order.EstimatedDeliveryDays = dto.EstimatedDeliveryDays;

        // Si la commande n'est pas encore expédiée, la marquer comme expédiée
        if (order.Status == OrderStatus.Processing)
        {
            order.Status = OrderStatus.Shipped;
            order.ShippedAt = DateTime.UtcNow;
            order.EstimatedDeliveryDate = DateTime.UtcNow.AddDays(dto.EstimatedDeliveryDays);
        }

        await _orderRepository.UpdateAsync(order);

        return new TrackingInfoDto(
            order.TrackingNumber,
            order.CarrierName,
            order.ShippedAt,
            order.EstimatedDeliveryDate,
            order.DeliveredAt,
            order.IsDeliveryDelayed
        );
    }

    public async Task<List<OrderDto>> GetDelayedOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        var delayedOrders = orders.Where(o => o.IsDeliveryDelayed).ToList();

        return delayedOrders.Select(MapOrderToDto).ToList();
    }

    private static ShippingMethodDto MapToDto(ShippingMethod method)
    {
        return new ShippingMethodDto(
            method.Id,
            method.Name,
            method.Description,
            method.Price,
            method.MinDeliveryDays,
            method.MaxDeliveryDays,
            method.CarrierName,
            method.IsActive
        );
    }

    private static OrderDto MapOrderToDto(Order order)
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
