using System.Security.Claims;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartController(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            cart = await _cartRepository.CreateAsync(cart);
        }

        var cartDto = await MapToCartDto(cart);
        return Ok(cartDto);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem([FromBody] AddToCartDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

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
        var cartDto = await MapToCartDto(cart);
        return Ok(cartDto);
    }

    [HttpPut("items")]
    public async Task<ActionResult<CartDto>> UpdateItem([FromBody] UpdateCartItemDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null)
            return NotFound();

        var item = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
        if (item == null)
            return NotFound();

        if (dto.Quantity <= 0)
        {
            cart.Items.Remove(item);
        }
        else
        {
            item.Quantity = dto.Quantity;
        }

        await _cartRepository.UpdateAsync(cart);
        var cartDto = await MapToCartDto(cart);
        return Ok(cartDto);
    }

    [HttpDelete("items/{productId}")]
    public async Task<ActionResult<CartDto>> RemoveItem(string productId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null)
            return NotFound();

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Items.Remove(item);
            await _cartRepository.UpdateAsync(cart);
        }

        var cartDto = await MapToCartDto(cart);
        return Ok(cartDto);
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart != null)
        {
            cart.Items.Clear();
            await _cartRepository.UpdateAsync(cart);
        }

        return NoContent();
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
