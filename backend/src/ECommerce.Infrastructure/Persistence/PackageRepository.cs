using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence;

public class PackageRepository : IPackageRepository
{
    private readonly IMongoCollection<Package> _packages;

    public PackageRepository(IMongoDatabase database)
    {
        _packages = database.GetCollection<Package>("packages");
    }

    public async Task<Package?> GetByIdAsync(string id)
    {
        return await _packages.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Package?> GetByOrderIdAsync(string orderId)
    {
        return await _packages.Find(p => p.OrderId == orderId).FirstOrDefaultAsync();
    }

    public async Task<List<Package>> GetAllAsync()
    {
        return await _packages
            .Find(_ => true)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Package>> GetByStatusAsync(PackageStatus status)
    {
        return await _packages.Find(p => p.Status == status).ToListAsync();
    }

    public async Task<List<Package>> GetPendingPackagesAsync()
    {
        return await _packages
            .Find(p => p.Status == PackageStatus.Pending || p.Status == PackageStatus.Preparing)
            .SortBy(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Package>> GetReadyToShipPackagesAsync()
    {
        return await _packages
            .Find(p => p.Status == PackageStatus.ReadyToShip)
            .SortBy(p => p.PreparedAt)
            .ToListAsync();
    }

    public async Task<Package> CreateAsync(Package package)
    {
        await _packages.InsertOneAsync(package);
        return package;
    }

    public async Task<Package> UpdateAsync(Package package)
    {
        await _packages.ReplaceOneAsync(p => p.Id == package.Id, package);
        return package;
    }

    public async Task DeleteAsync(string id)
    {
        await _packages.DeleteOneAsync(p => p.Id == id);
    }
}
