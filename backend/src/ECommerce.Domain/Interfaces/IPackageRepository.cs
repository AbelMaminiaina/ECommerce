using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IPackageRepository
{
    Task<Package?> GetByIdAsync(string id);
    Task<Package?> GetByOrderIdAsync(string orderId);
    Task<List<Package>> GetAllAsync();
    Task<List<Package>> GetByStatusAsync(PackageStatus status);
    Task<List<Package>> GetPendingPackagesAsync();
    Task<List<Package>> GetReadyToShipPackagesAsync();
    Task<Package> CreateAsync(Package package);
    Task<Package> UpdateAsync(Package package);
    Task DeleteAsync(string id);
}
