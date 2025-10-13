using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IShippingMethodRepository : IRepository<ShippingMethod>
{
    Task<IEnumerable<ShippingMethod>> GetActiveMethodsAsync();
}
