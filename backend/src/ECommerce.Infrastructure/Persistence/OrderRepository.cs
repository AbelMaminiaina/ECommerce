using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence;

public class OrderRepository : MongoRepository<Order>, IOrderRepository
{
    public OrderRepository(IMongoDatabase database)
        : base(database, "orders")
    {
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
    {
        return await _collection.Find(x => x.UserId == userId)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetByPaymentIntentIdAsync(string paymentIntentId)
    {
        return await _collection.Find(x => x.PaymentIntentId == paymentIntentId).FirstOrDefaultAsync();
    }
}
