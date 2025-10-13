using System.Security.Claims;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IPaymentService _paymentService;

    public OrdersController(
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // Si l'utilisateur est admin, récupérer toutes les commandes
        // Sinon, récupérer uniquement les commandes de l'utilisateur
        var orders = User.IsInRole("Admin")
            ? await _orderRepository.GetAllAsync()
            : await _orderRepository.GetByUserIdAsync(userId);

        var orderDtos = orders.Select(MapToDto);
        return Ok(orderDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var order = await _orderRepository.GetByIdAsync(id);

        if (order == null)
            return NotFound();

        if (order.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        return Ok(MapToDto(order));
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

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
            if (product == null || product.Stock < item.Quantity)
            {
                return BadRequest($"Product {item.ProductId} is not available");
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

        return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, MapToDto(createdOrder));
    }

    [HttpPost("{id}/payment")]
    public async Task<ActionResult<PaymentIntentResponseDto>> CreatePaymentIntent(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var order = await _orderRepository.GetByIdAsync(id);

        if (order == null)
            return NotFound();

        if (order.UserId != userId)
            return Forbid();

        try
        {
            var paymentIntent = await _paymentService.CreatePaymentIntentAsync(order.Id, order.TotalAmount);
            return Ok(paymentIntent);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(string id, [FromBody] OrderStatus status)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
            return NotFound();

        order.Status = status;

        // Si la commande est marquée comme livrée, enregistrer la date de livraison
        // pour démarrer le compte à rebours du droit de rétractation de 14 jours
        if (status == OrderStatus.Delivered && !order.DeliveredAt.HasValue)
        {
            order.DeliveredAt = DateTime.UtcNow;
        }

        await _orderRepository.UpdateAsync(order);

        return Ok(MapToDto(order));
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
