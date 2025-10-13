using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface ISupportTicketRepository : IRepository<SupportTicket>
{
    Task<IEnumerable<SupportTicket>> GetByUserIdAsync(string userId);
    Task<IEnumerable<SupportTicket>> GetByStatusAsync(TicketStatus status);
    Task<IEnumerable<SupportTicket>> GetOpenTicketsAsync();
}
