using System.Security.Claims;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReturnsController : ControllerBase
{
    private readonly IReturnService _returnService;

    public ReturnsController(IReturnService returnService)
    {
        _returnService = returnService;
    }

    /// <summary>
    /// Demander le retour d'une commande (Client)
    /// </summary>
    [HttpPost("orders/{orderId}/request")]
    public async Task<ActionResult<ReturnResponseDto>> RequestReturn(
        string orderId,
        [FromBody] RequestReturnDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var response = await _returnService.RequestReturnAsync(orderId, userId, dto);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtenir les informations de retour d'une commande
    /// </summary>
    [HttpGet("orders/{orderId}")]
    public async Task<ActionResult<ReturnResponseDto>> GetReturnInfo(string orderId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            var response = await _returnService.GetReturnInfoAsync(orderId, userId, isAdmin);
            return Ok(response);
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
    /// Mettre à jour le statut de retour (Admin uniquement)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("orders/{orderId}/status")]
    public async Task<ActionResult<ReturnResponseDto>> UpdateReturnStatus(
        string orderId,
        [FromBody] UpdateReturnStatusDto dto)
    {
        try
        {
            var response = await _returnService.UpdateReturnStatusAsync(orderId, dto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtenir toutes les demandes de retour (Admin uniquement)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllReturns()
    {
        try
        {
            var returns = await _returnService.GetAllReturnsAsync();
            return Ok(returns);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
