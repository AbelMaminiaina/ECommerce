using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Services.Carriers;

/// <summary>
/// Service pour l'intégration avec Chronopost
/// Documentation API : https://www.chronopost.fr/fr/page/integration-tracking-api
/// </summary>
public class ChronopostCarrierService : ICarrierService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;
    private readonly string _accountNumber;
    private readonly string _password;

    public ChronopostCarrierService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiUrl = configuration["Carriers:Chronopost:ApiUrl"] ?? "https://ws.chronopost.fr/shipping-cxf/ShippingServiceWS";
        _accountNumber = configuration["Carriers:Chronopost:AccountNumber"] ?? "";
        _password = configuration["Carriers:Chronopost:Password"] ?? "";
    }

    public bool SupportsCarrier(CarrierType carrier)
    {
        return carrier == CarrierType.Chronopost;
    }

    public async Task<ShippingLabelResponse> GenerateLabelAsync(ShippingLabelRequest request)
    {
        // TODO: Implémenter l'appel API réel à Chronopost
        // L'API Chronopost utilise également SOAP

        await Task.Delay(500);

        var trackingNumber = GenerateTrackingNumber();
        var estimatedDeliveryDate = DateTime.UtcNow.AddDays(1); // Chronopost = livraison express

        return new ShippingLabelResponse(
            TrackingNumber: trackingNumber,
            LabelUrl: $"https://example.com/labels/chronopost_{trackingNumber}.pdf",
            Cost: CalculateShippingCost(request),
            EstimatedDeliveryDate: estimatedDeliveryDate
        );
    }

    public async Task<TrackingInfoDto> GetTrackingInfoAsync(string trackingNumber)
    {
        await Task.Delay(300);

        return new TrackingInfoDto(
            TrackingNumber: trackingNumber,
            CarrierName: "Chronopost",
            ShippedAt: DateTime.UtcNow,
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(1),
            DeliveredAt: null,
            IsDelayed: false
        );
    }

    public async Task<bool> CancelShipmentAsync(string trackingNumber)
    {
        await Task.Delay(200);
        return true;
    }

    private string GenerateTrackingNumber()
    {
        // Format Chronopost : 13 chiffres
        var random = new Random();
        return string.Join("", Enumerable.Range(0, 13).Select(_ => random.Next(0, 10)));
    }

    private decimal CalculateShippingCost(ShippingLabelRequest request)
    {
        // Chronopost est plus cher (livraison express)
        if (request.Weight <= 0.5m) return 12.90m;
        if (request.Weight <= 1.0m) return 14.90m;
        if (request.Weight <= 2.0m) return 17.90m;
        if (request.Weight <= 5.0m) return 22.90m;
        if (request.Weight <= 10.0m) return 29.90m;
        return 39.90m;
    }
}
