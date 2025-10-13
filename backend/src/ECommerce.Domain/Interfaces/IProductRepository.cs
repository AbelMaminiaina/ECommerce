using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetFeaturedProductsAsync();
    Task<IEnumerable<Product>> GetByCategoryAsync(string categoryId);
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
}
