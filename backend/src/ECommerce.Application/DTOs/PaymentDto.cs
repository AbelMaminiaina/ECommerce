namespace ECommerce.Application.DTOs;

public record CreatePaymentIntentDto(
    string OrderId
);

public record PaymentIntentResponseDto(
    string ClientSecret,
    string PaymentIntentId
);
