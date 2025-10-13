using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByUserIdAsync(string userId);
}
