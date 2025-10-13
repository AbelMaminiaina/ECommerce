using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence;

public class SupportTicketRepository : MongoRepository<SupportTicket>, ISupportTicketRepository
{
    public SupportTicketRepository(IMongoDatabase database)
        : base(database, "supportTickets")
    {
    }

    public async Task<IEnumerable<SupportTicket>> GetByUserIdAsync(string userId)
    {
        return await _collection.Find(x => x.UserId == userId)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<SupportTicket>> GetByStatusAsync(TicketStatus status)
    {
        return await _collection.Find(x => x.Status == status)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<SupportTicket>> GetOpenTicketsAsync()
    {
        return await _collection.Find(x => x.Status != TicketStatus.Closed && x.Status != TicketStatus.Resolved)
            .SortByDescending(x => x.Priority)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
}
