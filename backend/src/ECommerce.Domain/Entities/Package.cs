namespace ECommerce.Domain.Entities;

/// <summary>
/// Représente un colis à préparer et expédier
/// </summary>
public class Package : BaseEntity
{
    public string OrderId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;

    // Informations du colis
    public decimal Weight { get; set; } // en kg
    public decimal Length { get; set; } // en cm
    public decimal Width { get; set; } // en cm
    public decimal Height { get; set; } // en cm

    // Statut de préparation
    public PackageStatus Status { get; set; } = PackageStatus.Pending;
    public DateTime? PreparedAt { get; set; }
    public string? PreparedBy { get; set; } // ID de l'admin qui a préparé le colis

    // Informations d'expédition
    public CarrierType Carrier { get; set; }
    public string? TrackingNumber { get; set; }
    public string? LabelUrl { get; set; } // URL de l'étiquette PDF
    public DateTime? ShippedAt { get; set; }

    // Point de collecte (pour Mondial Relay, etc.)
    public string? PickupPointId { get; set; }
    public string? PickupPointName { get; set; }
    public string? PickupPointAddress { get; set; }

    // Adresse de livraison
    public Address ShippingAddress { get; set; } = new();

    // Notifications
    public bool TrackingNotificationSent { get; set; } = false;
    public DateTime? TrackingNotificationSentAt { get; set; }

    // Notes internes
    public string? Notes { get; set; }
}

/// <summary>
/// Statut du colis
/// </summary>
public enum PackageStatus
{
    Pending,        // En attente de préparation
    Preparing,      // En cours de préparation
    ReadyToShip,    // Prêt à expédier (étiquette générée)
    Shipped,        // Expédié
    Delivered,      // Livré
    Exception,      // Problème d'expédition
    Returned        // Retourné
}

/// <summary>
/// Types de transporteurs supportés
/// </summary>
public enum CarrierType
{
    LaPoste,
    Colissimo,
    Chronopost,
    MondialRelay,
    DHL,
    UPS,
    FedEx
}
