namespace ECommerce.Application.DTOs;

public record CategoryDto(
    string Id,
    string Name,
    string Description,
    string? ParentCategoryId,
    string ImageUrl
);

public record CreateCategoryDto(
    string Name,
    string Description,
    string? ParentCategoryId,
    string ImageUrl
);
