using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence;

public class UserRepository : MongoRepository<User>, IUserRepository
{
    public UserRepository(IMongoDatabase database)
        : base(database, "users")
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _collection.Find(x => x.Email == email).FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsAsync(string email)
    {
        var count = await _collection.CountDocumentsAsync(x => x.Email == email);
        return count > 0;
    }
}
