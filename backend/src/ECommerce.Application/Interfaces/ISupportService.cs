using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces;

public interface ISupportService
{
    Task<List<TicketDto>> GetMyTicketsAsync(string userId);
    Task<List<TicketDto>> GetAllTicketsAsync();
    Task<TicketDto> GetTicketByIdAsync(string id, string userId, bool isAdmin);
    Task<TicketDto> CreateTicketAsync(string userId, CreateTicketDto dto);
    Task<TicketDto> AddMessageAsync(string ticketId, string userId, bool isAdmin, string message);
    Task<TicketDto> UpdateTicketStatusAsync(string ticketId, int status);
    Task<TicketDto> AssignTicketAsync(string ticketId, string adminId);
}
