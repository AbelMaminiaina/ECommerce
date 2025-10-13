using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence;

public class ShippingMethodRepository : MongoRepository<ShippingMethod>, IShippingMethodRepository
{
    public ShippingMethodRepository(IMongoDatabase database)
        : base(database, "shippingMethods")
    {
    }

    public async Task<IEnumerable<ShippingMethod>> GetActiveMethodsAsync()
    {
        return await _collection.Find(x => x.IsActive).ToListAsync();
    }
}
