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
public class SupportController : ControllerBase
{
    private readonly ISupportTicketRepository _ticketRepository;
    private readonly IUserRepository _userRepository;

    public SupportController(
        ISupportTicketRepository ticketRepository,
        IUserRepository userRepository)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
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

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Unauthorized();

        var ticket = new SupportTicket
        {
            UserId = userId,
            OrderId = dto.OrderId,
            Subject = dto.Subject,
            Description = dto.Description,
            Category = dto.Category,
            Messages = new List<TicketMessage>
            {
                new TicketMessage
                {
                    SenderId = userId,
                    SenderName = $"{user.FirstName} {user.LastName}",
                    IsFromAdmin = false,
                    Message = dto.Description
                }
            }
        };

        var createdTicket = await _ticketRepository.CreateAsync(ticket);

        return Ok(MapToDto(createdTicket));
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

        var tickets = await _ticketRepository.GetByUserIdAsync(userId);
        var ticketDtos = tickets.Select(MapToDto);

        return Ok(ticketDtos);
    }

    /// <summary>
    /// Obtenir un ticket spécifique
    /// </summary>
    [HttpGet("tickets/{ticketId}")]
    public async Task<ActionResult<TicketDto>> GetTicket(string ticketId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);

        if (ticket == null)
            return NotFound(new { message = "Ticket introuvable" });

        // Vérifier les permissions
        if (ticket.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        return Ok(MapToDto(ticket));
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

        var ticket = await _ticketRepository.GetByIdAsync(ticketId);

        if (ticket == null)
            return NotFound(new { message = "Ticket introuvable" });

        // Vérifier les permissions
        if (ticket.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Unauthorized();

        var message = new TicketMessage
        {
            SenderId = userId,
            SenderName = $"{user.FirstName} {user.LastName}",
            IsFromAdmin = User.IsInRole("Admin"),
            Message = dto.Message,
            Attachments = dto.Attachments ?? new List<string>()
        };

        ticket.Messages.Add(message);

        // Si admin répond, mettre le statut à InProgress
        if (User.IsInRole("Admin") && ticket.Status == TicketStatus.Open)
        {
            ticket.Status = TicketStatus.InProgress;
        }
        // Si client répond alors que le ticket était en attente, le remettre en InProgress
        else if (!User.IsInRole("Admin") && ticket.Status == TicketStatus.WaitingCustomer)
        {
            ticket.Status = TicketStatus.InProgress;
        }

        ticket.UpdatedAt = DateTime.UtcNow;
        await _ticketRepository.UpdateAsync(ticket);

        return Ok(MapToDto(ticket));
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
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);

        if (ticket == null)
            return NotFound();

        ticket.Status = dto.Status;

        if (dto.Priority.HasValue)
            ticket.Priority = dto.Priority.Value;

        if (!string.IsNullOrEmpty(dto.AssignedToAdminId))
            ticket.AssignedToAdminId = dto.AssignedToAdminId;

        if (dto.Status == TicketStatus.Closed || dto.Status == TicketStatus.Resolved)
            ticket.ClosedAt = DateTime.UtcNow;

        ticket.UpdatedAt = DateTime.UtcNow;
        await _ticketRepository.UpdateAsync(ticket);

        return Ok(MapToDto(ticket));
    }

    /// <summary>
    /// Obtenir tous les tickets ouverts (Admin uniquement)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("tickets/admin/open")]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetOpenTickets()
    {
        var tickets = await _ticketRepository.GetOpenTicketsAsync();
        var ticketDtos = tickets.Select(MapToDto);

        return Ok(ticketDtos);
    }

    /// <summary>
    /// Obtenir tous les tickets (Admin uniquement)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("tickets/admin/all")]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetAllTickets()
    {
        var tickets = await _ticketRepository.GetAllAsync();
        var ticketDtos = tickets.Select(MapToDto);

        return Ok(ticketDtos);
    }

    private static TicketDto MapToDto(SupportTicket ticket)
    {
        return new TicketDto(
            ticket.Id,
            ticket.UserId,
            ticket.OrderId,
            ticket.Subject,
            ticket.Description,
            ticket.Category,
            ticket.Status,
            ticket.Priority,
            ticket.Messages.Select(m => new TicketMessageDto(
                m.SenderId,
                m.SenderName,
                m.IsFromAdmin,
                m.Message,
                m.Attachments,
                m.CreatedAt
            )).ToList(),
            ticket.AssignedToAdminId,
            ticket.CreatedAt,
            ticket.ClosedAt
        );
    }
}
