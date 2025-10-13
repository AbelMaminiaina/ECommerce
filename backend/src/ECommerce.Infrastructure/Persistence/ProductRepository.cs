using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence;

public class ProductRepository : MongoRepository<Product>, IProductRepository
{
    public ProductRepository(IMongoDatabase database)
        : base(database, "products")
    {
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        return await _collection.Find(x => x.IsFeatured && x.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string categoryId)
    {
        return await _collection.Find(x => x.CategoryId == categoryId && x.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(x => x.IsActive, true),
            Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<Product>.Filter.Regex(x => x.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            )
        );
        return await _collection.Find(filter).ToListAsync();
    }
}
