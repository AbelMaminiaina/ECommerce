namespace ECommerce.Domain.Entities;

public class SupportTicket : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string? OrderId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketCategory Category { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public List<TicketMessage> Messages { get; set; } = new();
    public string? AssignedToAdminId { get; set; }
    public DateTime? ClosedAt { get; set; }
}

public class TicketMessage
{
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public bool IsFromAdmin { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Attachments { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum TicketCategory
{
    Order,          // Problème de commande
    Product,        // Problème produit
    Delivery,       // Problème livraison
    Return,         // Retour/Remboursement
    Technical,      // Problème technique
    Payment,        // Problème de paiement
    Other           // Autre
}

public enum TicketStatus
{
    Open,           // Ouvert
    InProgress,     // En cours
    WaitingCustomer,// En attente client
    Resolved,       // Résolu
    Closed          // Fermé
}

public enum TicketPriority
{
    Low,
    Medium,
    High,
    Urgent
}
