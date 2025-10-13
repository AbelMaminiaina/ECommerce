using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence;

public class CategoryRepository : MongoRepository<Category>, ICategoryRepository
{
    public CategoryRepository(IMongoDatabase database)
        : base(database, "categories")
    {
    }

    public async Task<IEnumerable<Category>> GetSubcategoriesAsync(string parentCategoryId)
    {
        return await _collection.Find(x => x.ParentCategoryId == parentCategoryId && x.IsActive).ToListAsync();
    }
}
