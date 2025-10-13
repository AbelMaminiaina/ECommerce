using ECommerce.Domain.Entities;

namespace ECommerce.Application.DTOs;

/// <summary>
/// DTO pour créer un ticket de support
/// </summary>
public record CreateTicketDto(
    string? OrderId,
    string Subject,
    string Description,
    TicketCategory Category
);

/// <summary>
/// DTO pour ajouter un message à un ticket
/// </summary>
public record AddTicketMessageDto(
    string Message,
    List<string>? Attachments
);

/// <summary>
/// DTO pour la réponse d'un ticket
/// </summary>
public record TicketDto(
    string Id,
    string UserId,
    string? OrderId,
    string Subject,
    string Description,
    TicketCategory Category,
    TicketStatus Status,
    TicketPriority Priority,
    List<TicketMessageDto> Messages,
    string? AssignedToAdminId,
    DateTime CreatedAt,
    DateTime? ClosedAt
);

/// <summary>
/// DTO pour un message de ticket
/// </summary>
public record TicketMessageDto(
    string SenderId,
    string SenderName,
    bool IsFromAdmin,
    string Message,
    List<string> Attachments,
    DateTime CreatedAt
);

/// <summary>
/// DTO pour mettre à jour le statut d'un ticket (Admin)
/// </summary>
public record UpdateTicketStatusDto(
    TicketStatus Status,
    TicketPriority? Priority,
    string? AssignedToAdminId
);
