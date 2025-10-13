namespace ECommerce.Application.DTOs;

/// <summary>
/// DTO pour les informations de suivi de livraison
/// </summary>
public record TrackingInfoDto(
    string? TrackingNumber,
    string? CarrierName,
    DateTime? ShippedAt,
    DateTime? EstimatedDeliveryDate,
    DateTime? DeliveredAt,
    bool IsDelayed
);

/// <summary>
/// DTO pour mettre à jour les informations d'expédition (Admin)
/// </summary>
public record UpdateShippingDto(
    string TrackingNumber,
    string CarrierName,
    int EstimatedDeliveryDays
);

/// <summary>
/// DTO pour les méthodes de livraison
/// </summary>
public record ShippingMethodDto(
    string Id,
    string Name,
    string Description,
    decimal Price,
    int MinDeliveryDays,
    int MaxDeliveryDays,
    string CarrierName,
    bool IsActive
);
