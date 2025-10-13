namespace ECommerce.Domain.Entities;

public class WarrantyClaim : BaseEntity
{
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public DateTime WarrantyExpirationDate { get; set; }
    public string IssueDescription { get; set; } = string.Empty;
    public List<string> Photos { get; set; } = new();
    public WarrantyClaimStatus Status { get; set; } = WarrantyClaimStatus.Submitted;
    public string? Resolution { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public enum WarrantyClaimStatus
{
    Submitted,      // Soumis
    UnderReview,    // En examen
    Approved,       // Approuvé
    Rejected,       // Rejeté
    Resolved        // Résolu (réparé/remplacé/remboursé)
}
