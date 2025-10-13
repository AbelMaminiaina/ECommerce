using ECommerce.Domain.Entities;

namespace ECommerce.Application.DTOs;

/// <summary>
/// DTO pour demander un retour de commande
/// </summary>
public record RequestReturnDto(
    string Reason
);

/// <summary>
/// DTO pour la réponse de retour
/// </summary>
public record ReturnResponseDto(
    string OrderId,
    ReturnStatus Status,
    string Reason,
    DateTime RequestedAt,
    DateTime? ReturnDeadline
);

/// <summary>
/// DTO pour mettre à jour le statut de retour (Admin)
/// </summary>
public record UpdateReturnStatusDto(
    ReturnStatus Status,
    string? AdminNotes
);
