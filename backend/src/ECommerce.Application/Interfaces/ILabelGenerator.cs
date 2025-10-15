using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces;

public interface ILabelGenerator
{
    byte[] GenerateLabel(
        string trackingNumber,
        CarrierType carrier,
        Address fromAddress,
        Address toAddress,
        decimal weight,
        string orderReference);
}
