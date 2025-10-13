using System.Security.Claims;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShippingController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IShippingMethodRepository _shippingMethodRepository;

    public ShippingController(
        IOrderRepository orderRepository,
        IShippingMethodRepository shippingMethodRepository)
    {
        _orderRepository = orderRepository;
        _shippingMethodRepository = shippingMethodRepository;
    }

    /// <summary>
    /// Obtenir le suivi d'une commande
    /// </summary>
    [Authorize]
    [HttpGet("tracking/{orderId}")]
    public async Task<ActionResult<TrackingInfoDto>> GetTracking(string orderId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            return NotFound(new { message = "Commande introuvable" });

        // Vérifier que la commande appartient à l'utilisateur (sauf admin)
        if (order.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        var tracking = new TrackingInfoDto(
            order.TrackingNumber,
            order.CarrierName,
            order.ShippedAt,
            order.EstimatedDeliveryDate,
            order.DeliveredAt,
            order.IsDeliveryDelayed
        );

        return Ok(tracking);
    }

    /// <summary>
    /// Mettre à jour les informations d'expédition (Admin uniquement)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("orders/{orderId}/shipping")]
    public async Task<ActionResult<TrackingInfoDto>> UpdateShipping(
        string orderId,
        [FromBody] UpdateShippingDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            return NotFound();

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

        var tracking = new TrackingInfoDto(
            order.TrackingNumber,
            order.CarrierName,
            order.ShippedAt,
            order.EstimatedDeliveryDate,
            order.DeliveredAt,
            order.IsDeliveryDelayed
        );

        return Ok(tracking);
    }

    /// <summary>
    /// Obtenir toutes les méthodes de livraison actives
    /// </summary>
    [HttpGet("methods")]
    public async Task<ActionResult<IEnumerable<ShippingMethodDto>>> GetShippingMethods()
    {
        var methods = await _shippingMethodRepository.GetActiveMethodsAsync();
        var methodDtos = methods.Select(m => new ShippingMethodDto(
            m.Id,
            m.Name,
            m.Description,
            m.Price,
            m.MinDeliveryDays,
            m.MaxDeliveryDays,
            m.CarrierName,
            m.IsActive
        ));

        return Ok(methodDtos);
    }

    /// <summary>
    /// Obtenir toutes les commandes en retard (Admin uniquement)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("delayed-orders")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetDelayedOrders()
    {
        var orders = await _orderRepository.GetAllAsync();
        var delayedOrders = orders.Where(o => o.IsDeliveryDelayed).ToList();

        var orderDtos = delayedOrders.Select(MapToDto);

        return Ok(orderDtos);
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
