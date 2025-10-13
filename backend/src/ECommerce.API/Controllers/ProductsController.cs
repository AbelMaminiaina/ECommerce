using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productRepository.GetAllAsync();
        var productDtos = products.Select(MapToDto);
        return Ok(productDtos);
    }

    [HttpGet("featured")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetFeatured()
    {
        var products = await _productRepository.GetFeaturedProductsAsync();
        var productDtos = products.Select(MapToDto);
        return Ok(productDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(string id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        return Ok(MapToDto(product));
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(string categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        var productDtos = products.Select(MapToDto);
        return Ok(productDtos);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Search([FromQuery] string term)
    {
        var products = await _productRepository.SearchProductsAsync(term);
        var productDtos = products.Select(MapToDto);
        return Ok(productDtos);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
            Images = dto.Images,
            IsFeatured = dto.IsFeatured,
            Specifications = dto.Specifications
        };

        var created = await _productRepository.CreateAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(string id, [FromBody] UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.CategoryId = dto.CategoryId;
        product.Images = dto.Images;
        product.IsFeatured = dto.IsFeatured;
        product.Specifications = dto.Specifications;

        var updated = await _productRepository.UpdateAsync(product);
        return Ok(MapToDto(updated));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _productRepository.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CategoryId,
            product.Images,
            product.IsFeatured,
            product.Specifications
        );
    }
}
