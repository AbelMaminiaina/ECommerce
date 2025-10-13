namespace ECommerce.Domain.Entities;

public class Cart : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = new();
}

public class CartItem
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
