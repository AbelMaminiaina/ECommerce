using ECommerce.Domain.Entities;

namespace ECommerce.Application.DTOs;

public record OrderDto(
    string Id,
    string UserId,
    List<OrderItemDto> Items,
    decimal TotalAmount,
    OrderStatus Status,
    AddressDto ShippingAddress,
    PaymentStatus PaymentStatus,
    DateTime CreatedAt,
    DateTime? DeliveredAt,
    DateTime? ReturnRequestedAt,
    string? ReturnReason,
    ReturnStatus ReturnStatus,
    DateTime? ReturnDeadline,
    bool CanReturn,
    DateTime? EstimatedDeliveryDate,
    DateTime? ShippedAt,
    string? TrackingNumber,
    string? CarrierName,
    int EstimatedDeliveryDays,
    bool IsDeliveryDelayed
);

public record OrderItemDto(
    string ProductId,
    string ProductName,
    int Quantity,
    decimal Price
);

public record CreateOrderDto(
    List<CreateOrderItemDto> Items,
    AddressDto ShippingAddress
);

public record CreateOrderItemDto(
    string ProductId,
    int Quantity
);
