using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(string userId);
    Task<CartDto> AddItemAsync(string userId, AddToCartDto dto);
    Task<CartDto> UpdateItemAsync(string userId, UpdateCartItemDto dto);
    Task<CartDto> RemoveItemAsync(string userId, string productId);
    Task ClearCartAsync(string userId);
}
