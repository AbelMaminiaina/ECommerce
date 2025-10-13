using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence;

public class WarrantyClaimRepository : MongoRepository<WarrantyClaim>, IWarrantyClaimRepository
{
    public WarrantyClaimRepository(IMongoDatabase database)
        : base(database, "warrantyClaims")
    {
    }

    public async Task<IEnumerable<WarrantyClaim>> GetByUserIdAsync(string userId)
    {
        return await _collection.Find(x => x.UserId == userId)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<WarrantyClaim?> GetByOrderAndProductAsync(string orderId, string productId)
    {
        return await _collection.Find(x => x.OrderId == orderId && x.ProductId == productId)
            .FirstOrDefaultAsync();
    }
}
