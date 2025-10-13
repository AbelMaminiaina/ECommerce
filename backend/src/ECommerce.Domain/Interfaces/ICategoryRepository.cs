using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetSubcategoriesAsync(string parentCategoryId);
}
