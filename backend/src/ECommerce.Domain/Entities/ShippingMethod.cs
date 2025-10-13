namespace ECommerce.Domain.Entities;

public class ShippingMethod : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MinDeliveryDays { get; set; }
    public int MaxDeliveryDays { get; set; }
    public string CarrierName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
