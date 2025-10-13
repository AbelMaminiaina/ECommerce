namespace ECommerce.Application.DTOs;

public record CartDto(
    string Id,
    string UserId,
    List<CartItemDto> Items,
    decimal TotalAmount
);

public record CartItemDto(
    string ProductId,
    string ProductName,
    decimal Price,
    int Quantity,
    decimal Subtotal
);

public record AddToCartDto(
    string ProductId,
    int Quantity
);

public record UpdateCartItemDto(
    string ProductId,
    int Quantity
);
