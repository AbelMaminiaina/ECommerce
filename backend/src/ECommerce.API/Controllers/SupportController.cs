using System.Security.Claims;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SupportController : ControllerBase
{
    private readonly ISupportService _supportService;

    public SupportController(ISupportService supportService)
    {
        _supportService = supportService;
    }

    /// <summary>
    /// Créer un nouveau ticket de support
    /// </summary>
    [HttpPost("tickets")]
    public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] CreateTicketDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var ticket = await _supportService.CreateTicketAsync(userId, dto);
            return Ok(ticket);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtenir tous les tickets de l'utilisateur connecté
    /// </summary>
    [HttpGet("tickets")]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetMyTickets()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var tickets = await _supportService.GetMyTicketsAsync(userId);
        return Ok(tickets);
    }

    /// <summary>
    /// Obtenir un ticket spécifique
    /// </summary>
    [HttpGet("tickets/{ticketId}")]
    public async Task<ActionResult<TicketDto>> GetTicket(string ticketId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var isAdmin = User.IsInRole("Admin");
            var ticket = await _supportService.GetTicketByIdAsync(ticketId, userId, isAdmin);
            return Ok(ticket);
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
    /// Ajouter un message à un ticket
    /// </summary>
    [HttpPost("tickets/{ticketId}/messages")]
    public async Task<ActionResult<TicketDto>> AddMessage(
        string ticketId,
        [FromBody] AddTicketMessageDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var isAdmin = User.IsInRole("Admin");
            var ticket = await _supportService.AddMessageAsync(ticketId, userId, isAdmin, dto.Message);
            return Ok(ticket);
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
    /// Mettre à jour le statut d'un ticket (Admin uniquement)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("tickets/{ticketId}/status")]
    public async Task<ActionResult<TicketDto>> UpdateTicketStatus(
        string ticketId,
        [FromBody] UpdateTicketStatusDto dto)
    {
        try
        {
            var ticket = await _supportService.UpdateTicketStatusAsync(ticketId, (int)dto.Status);

            if (dto.Priority.HasValue || !string.IsNullOrEmpty(dto.AssignedToAdminId))
            {
                if (!string.IsNullOrEmpty(dto.AssignedToAdminId))
                {
                    ticket = await _supportService.AssignTicketAsync(ticketId, dto.AssignedToAdminId);
                }
            }

            return Ok(ticket);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtenir tous les tickets (Admin uniquement)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("tickets/admin/all")]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetAllTickets()
    {
        var tickets = await _supportService.GetAllTicketsAsync();
        return Ok(tickets);
    }
}
