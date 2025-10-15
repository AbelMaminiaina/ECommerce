using ECommerce.Domain.Entities;

namespace ECommerce.Application.DTOs;

/// <summary>
/// DTO pour créer un colis
/// </summary>
public record CreatePackageDto(
    string OrderId,
    decimal Weight,
    decimal Length,
    decimal Width,
    decimal Height,
    CarrierType Carrier,
    string? PickupPointId = null,
    string? Notes = null
);

/// <summary>
/// DTO pour mettre à jour un colis
/// </summary>
public record UpdatePackageDto(
    decimal? Weight = null,
    decimal? Length = null,
    decimal? Width = null,
    decimal? Height = null,
    CarrierType? Carrier = null,
    PackageStatus? Status = null,
    string? Notes = null
);

/// <summary>
/// DTO pour afficher les informations d'un colis
/// </summary>
public record PackageDto(
    string Id,
    string OrderId,
    decimal Weight,
    decimal Length,
    decimal Width,
    decimal Height,
    PackageStatus Status,
    CarrierType Carrier,
    string? TrackingNumber,
    string? LabelUrl,
    DateTime? PreparedAt,
    DateTime? ShippedAt,
    Address ShippingAddress,
    string? PickupPointName,
    bool TrackingNotificationSent,
    string? Notes,
    DateTime CreatedAt
);

/// <summary>
/// DTO pour générer une étiquette d'expédition
/// </summary>
public record GenerateLabelRequest(
    string PackageId,
    CarrierType Carrier
);

/// <summary>
/// DTO pour le résultat de génération d'étiquette
/// </summary>
public record GenerateLabelResponse(
    string TrackingNumber,
    string LabelUrl,
    DateTime EstimatedDeliveryDate
);

/// <summary>
/// Informations nécessaires pour créer une étiquette d'expédition
/// </summary>
public record ShippingLabelRequest(
    CarrierType Carrier,
    Address FromAddress,
    Address ToAddress,
    decimal Weight,
    decimal Length,
    decimal Width,
    decimal Height,
    string? PickupPointId = null,
    string Reference = ""
);

/// <summary>
/// Réponse de création d'étiquette par le transporteur
/// </summary>
public record ShippingLabelResponse(
    string TrackingNumber,
    string LabelUrl,
    decimal Cost,
    DateTime EstimatedDeliveryDate
);
