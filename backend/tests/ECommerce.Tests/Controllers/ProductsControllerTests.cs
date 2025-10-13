using ECommerce.API.Controllers;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ECommerce.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _controller = new ProductsController(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product
            {
                Id = "1",
                Name = "Product 1",
                Description = "Description 1",
                Price = 99.99m,
                Stock = 10,
                CategoryId = "cat1",
                Images = new List<string> { "image1.jpg" },
                IsFeatured = true,
                Specifications = new Dictionary<string, string> { { "Color", "Red" } }
            },
            new Product
            {
                Id = "2",
                Name = "Product 2",
                Description = "Description 2",
                Price = 149.99m,
                Stock = 5,
                CategoryId = "cat2",
                Images = new List<string> { "image2.jpg" },
                IsFeatured = false,
                Specifications = new Dictionary<string, string> { { "Color", "Blue" } }
            }
        };

        _productRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
        returnedProducts.Should().HaveCount(2);

        _productRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetFeatured_ShouldReturnOnlyFeaturedProducts()
    {
        // Arrange
        var featuredProducts = new List<Product>
        {
            new Product
            {
                Id = "1",
                Name = "Featured Product",
                Description = "Description",
                Price = 99.99m,
                Stock = 10,
                CategoryId = "cat1",
                Images = new List<string> { "image1.jpg" },
                IsFeatured = true,
                Specifications = new Dictionary<string, string>()
            }
        };

        _productRepositoryMock
            .Setup(x => x.GetFeaturedProductsAsync())
            .ReturnsAsync(featuredProducts);

        // Act
        var result = await _controller.GetFeatured();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
        returnedProducts.Should().HaveCount(1);
        returnedProducts.First().IsFeatured.Should().BeTrue();

        _productRepositoryMock.Verify(x => x.GetFeaturedProductsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetById_WithExistingId_ShouldReturnOkWithProduct()
    {
        // Arrange
        var productId = "test-product-id";
        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Stock = 10,
            CategoryId = "cat1",
            Images = new List<string> { "image1.jpg" },
            IsFeatured = true,
            Specifications = new Dictionary<string, string> { { "Color", "Red" } }
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(productId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedProduct.Id.Should().Be(productId);
        returnedProduct.Name.Should().Be(product.Name);

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var productId = "non-existing-id";

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.GetById(productId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetByCategory_ShouldReturnProductsInCategory()
    {
        // Arrange
        var categoryId = "cat1";
        var products = new List<Product>
        {
            new Product
            {
                Id = "1",
                Name = "Product 1",
                Description = "Description",
                Price = 99.99m,
                Stock = 10,
                CategoryId = categoryId,
                Images = new List<string> { "image1.jpg" },
                IsFeatured = false,
                Specifications = new Dictionary<string, string>()
            }
        };

        _productRepositoryMock
            .Setup(x => x.GetByCategoryAsync(categoryId))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetByCategory(categoryId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
        returnedProducts.Should().HaveCount(1);
        returnedProducts.First().CategoryId.Should().Be(categoryId);

        _productRepositoryMock.Verify(x => x.GetByCategoryAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task Search_WithSearchTerm_ShouldReturnMatchingProducts()
    {
        // Arrange
        var searchTerm = "laptop";
        var products = new List<Product>
        {
            new Product
            {
                Id = "1",
                Name = "Gaming Laptop",
                Description = "High-performance laptop",
                Price = 1299.99m,
                Stock = 5,
                CategoryId = "electronics",
                Images = new List<string> { "laptop.jpg" },
                IsFeatured = true,
                Specifications = new Dictionary<string, string>()
            }
        };

        _productRepositoryMock
            .Setup(x => x.SearchProductsAsync(searchTerm))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.Search(searchTerm);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
        returnedProducts.Should().HaveCount(1);

        _productRepositoryMock.Verify(x => x.SearchProductsAsync(searchTerm), Times.Once);
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateProductDto(
            "New Product",
            "New Description",
            199.99m,
            20,
            "cat1",
            new List<string> { "new-image.jpg" },
            true,
            new Dictionary<string, string> { { "Color", "Green" } }
        );

        _productRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) =>
            {
                p.Id = "new-product-id";
                p.CreatedAt = DateTime.UtcNow;
                p.UpdatedAt = DateTime.UtcNow;
                return p;
            });

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(_controller.GetById));
        var returnedProduct = createdResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedProduct.Name.Should().Be(createDto.Name);
        returnedProduct.Price.Should().Be(createDto.Price);

        _productRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Update_WithExistingId_ShouldReturnOkWithUpdatedProduct()
    {
        // Arrange
        var productId = "existing-product-id";
        var existingProduct = new Product
        {
            Id = productId,
            Name = "Old Name",
            Description = "Old Description",
            Price = 99.99m,
            Stock = 10,
            CategoryId = "cat1",
            Images = new List<string> { "old-image.jpg" },
            IsFeatured = false,
            Specifications = new Dictionary<string, string>()
        };

        var updateDto = new UpdateProductDto(
            "Updated Name",
            "Updated Description",
            149.99m,
            15,
            "cat2",
            new List<string> { "updated-image.jpg" },
            true,
            new Dictionary<string, string> { { "Color", "Blue" } }
        );

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        // Act
        var result = await _controller.Update(productId, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedProduct.Name.Should().Be(updateDto.Name);
        returnedProduct.Price.Should().Be(updateDto.Price);
        returnedProduct.IsFeatured.Should().BeTrue();

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Update_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var productId = "non-existing-id";
        var updateDto = new UpdateProductDto(
            "Updated Name",
            "Updated Description",
            149.99m,
            15,
            "cat2",
            new List<string> { "updated-image.jpg" },
            true,
            new Dictionary<string, string>()
        );

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.Update(productId, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Delete_WithExistingId_ShouldReturnNoContent()
    {
        // Arrange
        var productId = "existing-product-id";

        _productRepositoryMock
            .Setup(x => x.DeleteAsync(productId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(productId);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        _productRepositoryMock.Verify(x => x.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task Delete_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var productId = "non-existing-id";

        _productRepositoryMock
            .Setup(x => x.DeleteAsync(productId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(productId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        _productRepositoryMock.Verify(x => x.DeleteAsync(productId), Times.Once);
    }
}
