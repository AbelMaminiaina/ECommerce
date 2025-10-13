namespace ECommerce.Application.DTOs;

public record ProductDto(
    string Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string CategoryId,
    List<string> Images,
    bool IsFeatured,
    Dictionary<string, string> Specifications
);

public record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string CategoryId,
    List<string> Images,
    bool IsFeatured,
    Dictionary<string, string> Specifications
);

public record UpdateProductDto(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string CategoryId,
    List<string> Images,
    bool IsFeatured,
    Dictionary<string, string> Specifications
);
