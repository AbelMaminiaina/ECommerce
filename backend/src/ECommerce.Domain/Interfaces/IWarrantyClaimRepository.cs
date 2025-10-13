using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IWarrantyClaimRepository : IRepository<WarrantyClaim>
{
    Task<IEnumerable<WarrantyClaim>> GetByUserIdAsync(string userId);
    Task<WarrantyClaim?> GetByOrderAndProductAsync(string orderId, string productId);
}
