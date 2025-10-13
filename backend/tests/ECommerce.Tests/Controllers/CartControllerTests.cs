using System.Security.Claims;
using ECommerce.API.Controllers;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ECommerce.Tests.Controllers;

public class CartControllerTests
{
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly CartController _controller;
    private const string TestUserId = "test-user-id";

    public CartControllerTests()
    {
        _cartRepositoryMock = new Mock<ICartRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _controller = new CartController(_cartRepositoryMock.Object, _productRepositoryMock.Object);

        // Setup User Claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, TestUserId)
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };
    }

    [Fact]
    public async Task GetCart_WithExistingCart_ShouldReturnOkWithCart()
    {
        // Arrange
        var cart = new Cart
        {
            Id = "cart-id",
            UserId = TestUserId,
            Items = new List<CartItem>
            {
                new CartItem { ProductId = "product-1", Quantity = 2 }
            }
        };

        var product = new Product
        {
            Id = "product-1",
            Name = "Test Product",
            Price = 99.99m,
            Stock = 10
        };

        _cartRepositoryMock
            .Setup(x => x.GetByUserIdAsync(TestUserId))
            .ReturnsAsync(cart);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync("product-1"))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetCart();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCart = okResult.Value.Should().BeOfType<CartDto>().Subject;
        returnedCart.UserId.Should().Be(TestUserId);
        returnedCart.Items.Should().HaveCount(1);
        returnedCart.TotalAmount.Should().Be(199.98m);

        _cartRepositoryMock.Verify(x => x.GetByUserIdAsync(TestUserId), Times.Once);
    }

    [Fact]
    public async Task GetCart_WithoutExistingCart_ShouldCreateAndReturnNewCart()
    {
        // Arrange
        _cartRepositoryMock
            .Setup(x => x.GetByUserIdAsync(TestUserId))
            .ReturnsAsync((Cart?)null);

        _cartRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Cart>()))
            .ReturnsAsync((Cart c) =>
            {
                c.Id = "new-cart-id";
                return c;
            });

        // Act
        var result = await _controller.GetCart();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCart = okResult.Value.Should().BeOfType<CartDto>().Subject;
        returnedCart.UserId.Should().Be(TestUserId);
        returnedCart.Items.Should().BeEmpty();
        returnedCart.TotalAmount.Should().Be(0);

        _cartRepositoryMock.Verify(x => x.GetByUserIdAsync(TestUserId), Times.Once);
        _cartRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Cart>()), Times.Once);
    }

    [Fact]
    public async Task AddItem_ToNewCart_ShouldCreateCartAndAddItem()
    {
        // Arrange
        var addToCartDto = new AddToCartDto("product-1", 2);

        _cartRepositoryMock
            .Setup(x => x.GetByUserIdAsync(TestUserId))
            .ReturnsAsync((Cart?)null);

        _cartRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Cart>()))
            .ReturnsAsync((Cart c) =>
            {
                c.Id = "new-cart-id";
                return c;
            });

        _cartRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Cart>()))
            .ReturnsAsync((Cart c) => c);

        var product = new Product
        {
            Id = "product-1",
            Name = "Test Product",
            Price = 99.99m,
            Stock = 10
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync("product-1"))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.AddItem(addToCartDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCart = okResult.Value.Should().BeOfType<CartDto>().Subject;
        returnedCart.Items.Should().HaveCount(1);
        returnedCart.Items.First().ProductId.Should().Be("product-1");
        returnedCart.Items.First().Quantity.Should().Be(2);

        _cartRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Cart>()), Times.Once);
        _cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>()), Times.Once);
    }

    [Fact]
    public async Task AddItem_ToExistingCartWithNewProduct_ShouldAddNewItem()
    {
        // Arrange
        var cart = new Cart
        {
            Id = "cart-id",
            UserId = TestUserId,
            Items = new List<CartItem>
            {
                new CartItem { ProductId = "product-1", Quantity = 1 }
            }
        };

        var addToCartDto = new AddToCartDto("product-2", 3);

        _cartRepositoryMock
            .Setup(x => x.GetByUserIdAsync(TestUserId))
            .ReturnsAsync(cart);

        _cartRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Cart>()))
            .ReturnsAsync((Cart c) => c);

        var product1 = new Product { Id = "product-1", Name = "Product 1", Price = 50m, Stock = 10 };
        var product2 = new Product { Id = "product-2", Name = "Product 2", Price = 75m, Stock = 10 };

        _productRepositoryMock.Setup(x => x.GetByIdAsync("product-1")).ReturnsAsync(product1);
        _productRepositoryMock.Setup(x => x.GetByIdAsync("product-2")).ReturnsAsync(product2);

        // Act
        var result = await _controller.AddItem(addToCartDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCart = okResult.Value.Should().BeOfType<CartDto>().Subject;
        returnedCart.Items.Should().HaveCount(2);

        _cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>()), Times.Once);
    }

    [Fact]
    public async Task AddItem_ToExistingCartWithSameProduct_ShouldIncreaseQuantity()
    {
        // Arrange
        var cart = new Cart
        {
            Id = "cart-id",
            UserId = TestUserId,
            Items = new List<CartItem>
            {
                new CartItem { ProductId = "product-1", Quantity = 2 }
            }
        };

        var addToCartDto = new AddToCartDto("product-1", 3);

        _cartRepositoryMock
            .Setup(x => x.GetByUserIdAsync(TestUserId))
            .ReturnsAsync(cart);

        _cartRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Cart>()))
            .ReturnsAsync((Cart c) => c);

        var product = new Product { Id = "product-1", Name = "Product 1", Price = 50m, Stock = 10 };
        _productRepositoryMock.Setup(x => x.GetByIdAsync("product-1")).ReturnsAsync(product);

        // Act
        var result = await _controller.AddItem(addToCartDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCart = okResult.Value.Should().BeOfType<CartDto>().Subject;
        returnedCart.Items.Should().HaveCount(1);
        returnedCart.Items.First().Quantity.Should().Be(5); // 2 + 3

        _cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>()), Times.Once);
    }

    [Fact]
    public async Task UpdateItem_WithValidQuantity_ShouldUpdateItemQuantity()
    {
        // Arrange
        var cart = new Cart
        {
            Id = "cart-id",
            UserId = TestUserId,
            Items = new List<CartItem>
            {
                new CartItem { ProductId = "product-1", Quantity = 2 }
            }
        };

        var updateDto = new UpdateCartItemDto("product-1", 5);

        _cartRepositoryMock
            .Setup(x => x.GetByUserIdAsync(TestUserId))
            .ReturnsAsync(cart);

        _cartRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Cart>()))
            .ReturnsAsync((Cart c) => c);

        var product = new Product { Id = "product-1", Name = "Product 1", Price = 50m, Stock = 10 };
        _productRepositoryMock.Setup(x => x.GetByIdAsync("product-1")).ReturnsAsync(product);

        // Act
        var result = await _controller.UpdateItem(updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCart = okResult.Value.Should().BeOfType<CartDto>().Subject;
        returnedCart.Items.First().Quantity.Should().Be(5);

        _cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>()), Times.Once);
    }

    [Fact]
    public async Task UpdateItem_WithZeroQuantity_ShouldRemoveItem()
    {
        // Arrange
        var cart = new Cart
        {
            Id = "cart-id",
            UserId = TestUserId,
            Items = new List<CartItem>
            {
                new CartItem { ProductId = "product-1", Quantity = 2 },
                new CartItem { ProductId = "product-2", Quantity = 1 }
            }
        };

        var updateDto = new UpdateCartItemDto("product-1", 0);

        _cartRepositoryMock
            .Setup(x => x.GetByUserIdAsync(TestUserId))
            .ReturnsAsync(cart);

        _cartRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Cart>()))
            .ReturnsAsync((Cart c) => c);

        var product = new Product { Id = "product-2", Name = "Product 2", Price = 75m, Stock = 10 };
        _productRepositoryMock.Setup(x => x.GetByIdAsync("product-2")).ReturnsAsync(product);

        // Act
        var result = await _controller.UpdateItem(updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCart = okResult.Value.Should().BeOfType<CartDto>().Subject;
        returnedCart.Items.Should().HaveCount(1);
        returnedCart.Items.Should().NotContain(i => i.ProductId == "product-1");

        _cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>()), Times.Once);
    }

    [Fact]
    public async Task UpdateItem_WithNonExistentCart_ShouldReturnNotFound()
    {
        // Arrange
        var updateDto = new UpdateCartItemDto("product-1", 5);

        _cartRepositoryMock
            .Setup(x => x.GetByUserIdAsync(TestUserId))
            .ReturnsAsync((Cart?)null);

        // Act
        var result = await _controller.UpdateItem(updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();

        _cartRepositoryMock.Verify(x => x.GetByUserIdAsync(TestUserId), Times.Once);
        _cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>()), Times.Never);
    }

    [Fact]
    public async Task RemoveItem_WithExistingItem_ShouldRemoveItemFromCart()
    {
        // Arrange
        var productId = "product-1";
        var cart = new Cart
        {
            Id = "cart-id",
            UserId = TestUserId,
            Items = new List<CartItem>
            {
                new CartItem { ProductId = "product-1", Quantity = 2 },
                new CartItem { ProductId = "product-2", Quantity = 1 }
            }
        };

        _cartRepositoryMock
            .Setup(x => x.GetByUserIdAsync(TestUserId))
            .ReturnsAsync(cart);

        _cartRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Cart>()))
            .ReturnsAsync((Cart c) => c);

        var product = new Product { Id = "product-2", Name = "Product 2", Price = 75m, Stock = 10 };
        _productRepositoryMock.Setup(x => x.GetByIdAsync("product-2")).ReturnsAsync(product);

        // Act
        var result = await _controller.RemoveItem(productId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCart = okResult.Value.Should().BeOfType<CartDto>().Subject;
        returnedCart.Items.Should().HaveCount(1);
        returnedCart.Items.Should().NotContain(i => i.ProductId == productId);

        _cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>()), Times.Once);
    }

    [Fact]
    public async Task RemoveItem_WithNonExistentCart_ShouldReturnNotFound()
    {
        // Arrange
        var productId = "product-1";

        _cartRepositoryMock
            .Setup(x => x.GetByUserIdAsync(TestUserId))
            .ReturnsAsync((Cart?)null);

        // Act
        var result = await _controller.RemoveItem(productId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();

        _cartRepositoryMock.Verify(x => x.GetByUserIdAsync(TestUserId), Times.Once);
        _cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>()), Times.Never);
    }

    [Fact]
    public async Task ClearCart_WithExistingCart_ShouldRemoveAllItems()
    {
        // Arrange
        var cart = new Cart
        {
            Id = "cart-id",
            UserId = TestUserId,
            Items = new List<CartItem>
            {
                new CartItem { ProductId = "product-1", Quantity = 2 },
                new CartItem { ProductId = "product-2", Quantity = 1 }
            }
        };

        _cartRepositoryMock
            .Setup(x => x.GetByUserIdAsync(TestUserId))
            .ReturnsAsync(cart);

        _cartRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Cart>()))
            .ReturnsAsync((Cart c) => c);

        // Act
        var result = await _controller.ClearCart();

        // Assert
        result.Should().BeOfType<NoContentResult>();

        _cartRepositoryMock.Verify(x => x.GetByUserIdAsync(TestUserId), Times.Once);
        _cartRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Cart>(c => c.Items.Count == 0)), Times.Once);
    }

    [Fact]
    public async Task ClearCart_WithoutExistingCart_ShouldReturnNoContent()
    {
        // Arrange
        _cartRepositoryMock
            .Setup(x => x.GetByUserIdAsync(TestUserId))
            .ReturnsAsync((Cart?)null);

        // Act
        var result = await _controller.ClearCart();

        // Assert
        result.Should().BeOfType<NoContentResult>();

        _cartRepositoryMock.Verify(x => x.GetByUserIdAsync(TestUserId), Times.Once);
        _cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>()), Times.Never);
    }
}
