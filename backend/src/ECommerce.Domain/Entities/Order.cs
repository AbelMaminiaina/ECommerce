namespace ECommerce.Domain.Entities;

public class Order : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public Address ShippingAddress { get; set; } = new();
    public string PaymentIntentId { get; set; } = string.Empty;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    // Propriétés pour le droit de rétractation (14 jours)
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReturnRequestedAt { get; set; }
    public string? ReturnReason { get; set; }
    public ReturnStatus ReturnStatus { get; set; } = ReturnStatus.None;

    // Propriétés pour le suivi de livraison
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? ShippedAt { get; set; }
    public string? TrackingNumber { get; set; }
    public string? CarrierName { get; set; }
    public int EstimatedDeliveryDays { get; set; } = 3;

    // Calcul de la date limite de retour (14 jours après livraison)
    public DateTime? ReturnDeadline => DeliveredAt?.AddDays(14);

    // Vérifier si le produit peut encore être retourné
    public bool CanReturn => DeliveredAt.HasValue &&
                             DateTime.UtcNow <= ReturnDeadline &&
                             ReturnStatus == ReturnStatus.None &&
                             Status == OrderStatus.Delivered;

    // Vérifier si la livraison est en retard
    public bool IsDeliveryDelayed => EstimatedDeliveryDate.HasValue &&
                                      DateTime.UtcNow > EstimatedDeliveryDate &&
                                      Status != OrderStatus.Delivered &&
                                      Status != OrderStatus.Cancelled;
}

public class OrderItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled,
    ReturnRequested,
    Returned
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}

public enum ReturnStatus
{
    None,           // Pas de retour demandé
    Requested,      // Retour demandé par le client
    Approved,       // Retour approuvé par l'admin
    InTransit,      // Produit en cours de retour
    Received,       // Produit retourné reçu
    Refunded,       // Remboursement effectué
    Rejected        // Retour refusé
}
