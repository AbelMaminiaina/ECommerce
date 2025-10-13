using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _categoryRepository.GetAllAsync();
        var categoryDtos = categories.Select(MapToDto);
        return Ok(categoryDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(string id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        return Ok(MapToDto(category));
    }

    [HttpGet("{id}/subcategories")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetSubcategories(string id)
    {
        var subcategories = await _categoryRepository.GetSubcategoriesAsync(id);
        var categoryDtos = subcategories.Select(MapToDto);
        return Ok(categoryDtos);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            ParentCategoryId = dto.ParentCategoryId,
            ImageUrl = dto.ImageUrl
        };

        var created = await _categoryRepository.CreateAsync(category);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _categoryRepository.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto(
            category.Id,
            category.Name,
            category.Description,
            category.ParentCategoryId,
            category.ImageUrl
        );
    }
}
