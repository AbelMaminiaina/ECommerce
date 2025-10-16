using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces;

public interface IProductService
{
    Task<List<ProductDto>> GetAllProductsAsync();
    Task<List<ProductDto>> GetFeaturedProductsAsync();
    Task<ProductDto> GetProductByIdAsync(string id);
    Task<List<ProductDto>> GetProductsByCategoryAsync(string categoryId);
    Task<List<ProductDto>> SearchProductsAsync(string term);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto> UpdateProductAsync(string id, UpdateProductDto dto);
    Task<bool> DeleteProductAsync(string id);
}
