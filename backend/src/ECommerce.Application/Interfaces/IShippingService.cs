using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces;

public interface IShippingService
{
    Task<List<ShippingMethodDto>> GetAllShippingMethodsAsync();
    Task<ShippingMethodDto> GetShippingMethodByIdAsync(string id);
    Task<ShippingMethodDto> CreateShippingMethodAsync(CreateShippingMethodDto dto);
    Task<ShippingMethodDto> UpdateShippingMethodAsync(string id, UpdateShippingMethodDto dto);
    Task DeleteShippingMethodAsync(string id);
    Task<List<ShippingMethodDto>> GetAvailableShippingMethodsAsync(string orderId, string userId);
    Task<TrackingInfoDto> GetTrackingAsync(string orderId, string userId, bool isAdmin);
    Task<TrackingInfoDto> UpdateShippingInfoAsync(string orderId, UpdateShippingDto dto);
    Task<List<OrderDto>> GetDelayedOrdersAsync();
}
