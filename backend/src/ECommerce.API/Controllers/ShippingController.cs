using System.Security.Claims;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShippingController : ControllerBase
{
    private readonly IShippingService _shippingService;

    public ShippingController(IShippingService shippingService)
    {
        _shippingService = shippingService;
    }

    /// <summary>
    /// Obtenir le suivi d'une commande
    /// </summary>
    [Authorize]
    [HttpGet("tracking/{orderId}")]
    public async Task<ActionResult<TrackingInfoDto>> GetTracking(string orderId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            var tracking = await _shippingService.GetTrackingAsync(orderId, userId, isAdmin);
            return Ok(tracking);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
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
        try
        {
            var tracking = await _shippingService.UpdateShippingInfoAsync(orderId, dto);
            return Ok(tracking);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtenir toutes les méthodes de livraison actives
    /// </summary>
    [HttpGet("methods")]
    public async Task<ActionResult<IEnumerable<ShippingMethodDto>>> GetShippingMethods()
    {
        var methods = await _shippingService.GetAllShippingMethodsAsync();
        return Ok(methods);
    }

    /// <summary>
    /// Obtenir toutes les commandes en retard (Admin uniquement)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("delayed-orders")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetDelayedOrders()
    {
        var orders = await _shippingService.GetDelayedOrdersAsync();
        return Ok(orders);
    }
}
