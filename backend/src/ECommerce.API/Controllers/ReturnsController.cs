using System.Security.Claims;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReturnsController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;

    public ReturnsController(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    /// <summary>
    /// Demander le retour d'une commande (Client)
    /// </summary>
    [HttpPost("orders/{orderId}/request")]
    public async Task<ActionResult<ReturnResponseDto>> RequestReturn(
        string orderId,
        [FromBody] RequestReturnDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            return NotFound(new { message = "Commande introuvable" });

        // Vérifier que la commande appartient à l'utilisateur
        if (order.UserId != userId)
            return Forbid();

        // Vérifier si le retour est possible
        if (!order.CanReturn)
        {
            if (order.ReturnStatus != ReturnStatus.None)
                return BadRequest(new { message = "Un retour a déjà été demandé pour cette commande" });

            if (order.Status != OrderStatus.Delivered)
                return BadRequest(new { message = "La commande doit être livrée pour être retournée" });

            if (!order.DeliveredAt.HasValue)
                return BadRequest(new { message = "La date de livraison n'est pas définie" });

            if (DateTime.UtcNow > order.ReturnDeadline)
                return BadRequest(new { message = $"Le délai de rétractation de 14 jours est dépassé. Date limite: {order.ReturnDeadline:dd/MM/yyyy}" });

            return BadRequest(new { message = "Le retour n'est pas possible pour cette commande" });
        }

        // Enregistrer la demande de retour
        order.ReturnRequestedAt = DateTime.UtcNow;
        order.ReturnReason = dto.Reason;
        order.ReturnStatus = ReturnStatus.Requested;
        order.Status = OrderStatus.ReturnRequested;

        await _orderRepository.UpdateAsync(order);

        var response = new ReturnResponseDto(
            order.Id,
            order.ReturnStatus,
            order.ReturnReason ?? "",
            order.ReturnRequestedAt.Value,
            order.ReturnDeadline
        );

        return Ok(response);
    }

    /// <summary>
    /// Obtenir les informations de retour d'une commande
    /// </summary>
    [HttpGet("orders/{orderId}")]
    public async Task<ActionResult<ReturnResponseDto>> GetReturnInfo(string orderId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            return NotFound();

        if (order.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        if (order.ReturnStatus == ReturnStatus.None)
            return NotFound(new { message = "Aucun retour demandé pour cette commande" });

        var response = new ReturnResponseDto(
            order.Id,
            order.ReturnStatus,
            order.ReturnReason ?? "",
            order.ReturnRequestedAt ?? DateTime.MinValue,
            order.ReturnDeadline
        );

        return Ok(response);
    }

    /// <summary>
    /// Mettre à jour le statut de retour (Admin uniquement)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("orders/{orderId}/status")]
    public async Task<ActionResult<ReturnResponseDto>> UpdateReturnStatus(
        string orderId,
        [FromBody] UpdateReturnStatusDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            return NotFound();

        if (order.ReturnStatus == ReturnStatus.None)
            return BadRequest(new { message = "Aucun retour n'a été demandé pour cette commande" });

        // Mettre à jour le statut
        order.ReturnStatus = dto.Status;

        // Si le retour est approuvé et le produit reçu, rembourser et marquer comme retourné
        if (dto.Status == ReturnStatus.Refunded)
        {
            order.Status = OrderStatus.Returned;
            order.PaymentStatus = PaymentStatus.Refunded;
        }
        else if (dto.Status == ReturnStatus.Rejected)
        {
            // Si rejeté, remettre en "Delivered"
            order.Status = OrderStatus.Delivered;
        }

        await _orderRepository.UpdateAsync(order);

        var response = new ReturnResponseDto(
            order.Id,
            order.ReturnStatus,
            order.ReturnReason ?? "",
            order.ReturnRequestedAt ?? DateTime.MinValue,
            order.ReturnDeadline
        );

        return Ok(response);
    }

    /// <summary>
    /// Obtenir toutes les demandes de retour (Admin uniquement)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReturnResponseDto>>> GetAllReturns()
    {
        var orders = await _orderRepository.GetAllAsync();
        var returns = orders
            .Where(o => o.ReturnStatus != ReturnStatus.None)
            .Select(o => new ReturnResponseDto(
                o.Id,
                o.ReturnStatus,
                o.ReturnReason ?? "",
                o.ReturnRequestedAt ?? DateTime.MinValue,
                o.ReturnDeadline
            ));

        return Ok(returns);
    }
}
