using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartDto> GetCartAsync(string userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            cart = await _cartRepository.CreateAsync(cart);
        }

        return await MapToCartDto(cart);
    }

    public async Task<CartDto> AddItemAsync(string userId, AddToCartDto dto)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            cart = await _cartRepository.CreateAsync(cart);
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            });
        }

        await _cartRepository.UpdateAsync(cart);
        return await MapToCartDto(cart);
    }

    public async Task<CartDto> UpdateItemAsync(string userId, UpdateCartItemDto dto)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null)
            throw new Exception("Panier introuvable");

        var item = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
        if (item == null)
            throw new Exception("Article introuvable dans le panier");

        if (dto.Quantity <= 0)
        {
            cart.Items.Remove(item);
        }
        else
        {
            item.Quantity = dto.Quantity;
        }

        await _cartRepository.UpdateAsync(cart);
        return await MapToCartDto(cart);
    }

    public async Task<CartDto> RemoveItemAsync(string userId, string productId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null)
            throw new Exception("Panier introuvable");

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Items.Remove(item);
            await _cartRepository.UpdateAsync(cart);
        }

        return await MapToCartDto(cart);
    }

    public async Task ClearCartAsync(string userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart != null)
        {
            cart.Items.Clear();
            await _cartRepository.UpdateAsync(cart);
        }
    }

    private async Task<CartDto> MapToCartDto(Cart cart)
    {
        var items = new List<CartItemDto>();
        decimal totalAmount = 0;

        foreach (var item in cart.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                var subtotal = product.Price * item.Quantity;
                items.Add(new CartItemDto(
                    item.ProductId,
                    product.Name,
                    product.Price,
                    item.Quantity,
                    subtotal
                ));
                totalAmount += subtotal;
            }
        }

        return new CartDto(cart.Id, cart.UserId, items, totalAmount);
    }
}
