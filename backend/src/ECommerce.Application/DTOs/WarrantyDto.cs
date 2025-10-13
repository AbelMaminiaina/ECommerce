using ECommerce.Domain.Entities;

namespace ECommerce.Application.DTOs;

/// <summary>
/// DTO pour soumettre une réclamation de garantie
/// </summary>
public record SubmitWarrantyClaimDto(
    string OrderId,
    string ProductId,
    string IssueDescription,
    List<string>? Photos
);

/// <summary>
/// DTO pour la réponse d'une réclamation
/// </summary>
public record WarrantyClaimDto(
    string Id,
    string OrderId,
    string ProductId,
    string ProductName,
    string UserId,
    DateTime PurchaseDate,
    DateTime WarrantyExpirationDate,
    string IssueDescription,
    List<string> Photos,
    WarrantyClaimStatus Status,
    string? Resolution,
    string? AdminNotes,
    DateTime CreatedAt,
    DateTime? ResolvedAt,
    bool IsUnderWarranty
);

/// <summary>
/// DTO pour mettre à jour une réclamation (Admin)
/// </summary>
public record UpdateWarrantyClaimDto(
    WarrantyClaimStatus Status,
    string? Resolution,
    string? AdminNotes
);
