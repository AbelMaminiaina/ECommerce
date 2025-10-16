using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services;

public class SupportService : ISupportService
{
    private readonly ISupportTicketRepository _ticketRepository;
    private readonly IUserRepository _userRepository;

    public SupportService(
        ISupportTicketRepository ticketRepository,
        IUserRepository userRepository)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
    }

    public async Task<List<TicketDto>> GetMyTicketsAsync(string userId)
    {
        var tickets = await _ticketRepository.GetByUserIdAsync(userId);
        return tickets.Select(MapToDto).ToList();
    }

    public async Task<List<TicketDto>> GetAllTicketsAsync()
    {
        var tickets = await _ticketRepository.GetAllAsync();
        return tickets.Select(MapToDto).ToList();
    }

    public async Task<TicketDto> GetTicketByIdAsync(string id, string userId, bool isAdmin)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket == null)
            throw new Exception("Ticket introuvable");

        if (ticket.UserId != userId && !isAdmin)
            throw new UnauthorizedAccessException("Accès non autorisé à ce ticket");

        return MapToDto(ticket);
    }

    public async Task<TicketDto> CreateTicketAsync(string userId, CreateTicketDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Utilisateur introuvable");

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
        return MapToDto(createdTicket);
    }

    public async Task<TicketDto> AddMessageAsync(string ticketId, string userId, bool isAdmin, string message)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            throw new Exception("Ticket introuvable");

        if (ticket.UserId != userId && !isAdmin)
            throw new UnauthorizedAccessException("Accès non autorisé à ce ticket");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Utilisateur introuvable");

        var ticketMessage = new TicketMessage
        {
            SenderId = userId,
            SenderName = $"{user.FirstName} {user.LastName}",
            IsFromAdmin = isAdmin,
            Message = message,
            Attachments = new List<string>()
        };

        ticket.Messages.Add(ticketMessage);

        if (isAdmin && ticket.Status == TicketStatus.Open)
        {
            ticket.Status = TicketStatus.InProgress;
        }
        else if (!isAdmin && ticket.Status == TicketStatus.WaitingCustomer)
        {
            ticket.Status = TicketStatus.InProgress;
        }

        ticket.UpdatedAt = DateTime.UtcNow;
        await _ticketRepository.UpdateAsync(ticket);

        return MapToDto(ticket);
    }

    public async Task<TicketDto> UpdateTicketStatusAsync(string ticketId, int status)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            throw new Exception("Ticket introuvable");

        ticket.Status = (TicketStatus)status;

        if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Resolved)
            ticket.ClosedAt = DateTime.UtcNow;

        ticket.UpdatedAt = DateTime.UtcNow;
        await _ticketRepository.UpdateAsync(ticket);

        return MapToDto(ticket);
    }

    public async Task<TicketDto> AssignTicketAsync(string ticketId, string adminId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            throw new Exception("Ticket introuvable");

        ticket.AssignedToAdminId = adminId;
        ticket.UpdatedAt = DateTime.UtcNow;
        await _ticketRepository.UpdateAsync(ticket);

        return MapToDto(ticket);
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
