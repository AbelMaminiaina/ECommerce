using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence;

public class CartRepository : MongoRepository<Cart>, ICartRepository
{
    public CartRepository(IMongoDatabase database)
        : base(database, "carts")
    {
    }

    public async Task<Cart?> GetByUserIdAsync(string userId)
    {
        return await _collection.Find(x => x.UserId == userId).FirstOrDefaultAsync();
    }
}
