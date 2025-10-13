namespace ECommerce.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string CategoryId { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;
    public Dictionary<string, string> Specifications { get; set; } = new();

    // Propriétés pour la garantie légale de conformité
    public int WarrantyMonths { get; set; } = 24; // 2 ans par défaut (garantie légale)
    public string WarrantyType { get; set; } = "Légale"; // Légale, Constructeur, Étendue
    public bool IsNew { get; set; } = true; // Produit neuf ou occasion
}
