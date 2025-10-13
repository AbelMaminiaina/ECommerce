using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Payment;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace ECommerce.Tests.Services;

public class StripePaymentServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly StripeSettings _stripeSettings;
    private readonly IPaymentService _paymentService;

    public StripePaymentServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _stripeSettings = new StripeSettings
        {
            SecretKey = "test_secret_key_for_unit_tests",
            PublishableKey = "test_publishable_key_for_unit_tests"
        };
        var stripeOptions = Options.Create(_stripeSettings);
        _paymentService = new StripePaymentService(_orderRepositoryMock.Object, stripeOptions);
    }

    [Fact]
    public async Task CreatePaymentIntentAsync_WithNonExistentOrder_ShouldThrowException()
    {
        // Arrange
        var orderId = "non-existent-order-id";
        var amount = 100.50m;

        _orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync((Order?)null);

        // Act
        Func<Task> act = async () => await _paymentService.CreatePaymentIntentAsync(orderId, amount);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Order not found");

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task ConfirmPaymentAsync_WithNonExistentOrder_ShouldReturnFalse()
    {
        // Arrange
        var paymentIntentId = "pi_test_123456";

        _orderRepositoryMock
            .Setup(x => x.GetByPaymentIntentIdAsync(paymentIntentId))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _paymentService.ConfirmPaymentAsync(paymentIntentId);

        // Assert
        result.Should().BeFalse();
        _orderRepositoryMock.Verify(x => x.GetByPaymentIntentIdAsync(paymentIntentId), Times.Once);
    }

    // Note: Les tests suivants nécessiteraient de mocker le service Stripe,
    // ce qui requiert une refactorisation du code pour injecter PaymentIntentService.
    // Pour des tests complets, considérez:
    // 1. Refactorer StripePaymentService pour accepter IPaymentIntentService
    // 2. Créer une interface wrapper pour PaymentIntentService
    // 3. Utiliser des tests d'intégration avec Stripe Test Mode

    [Fact]
    public void Constructor_ShouldSetStripeApiKey()
    {
        // Arrange & Act
        var service = new StripePaymentService(_orderRepositoryMock.Object, Options.Create(_stripeSettings));

        // Assert
        service.Should().NotBeNull();
        Stripe.StripeConfiguration.ApiKey.Should().Be(_stripeSettings.SecretKey);
    }
}
