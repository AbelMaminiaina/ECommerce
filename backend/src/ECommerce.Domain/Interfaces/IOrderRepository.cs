using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
    Task<Order?> GetByPaymentIntentIdAsync(string paymentIntentId);
}
