using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Services.Carriers;

/// <summary>
/// Service pour l'intégration avec DHL
/// Documentation API : https://developer.dhl.com/api-reference/shipment
/// </summary>
public class DHLCarrierService : ICarrierService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;
    private readonly string _apiKey;
    private readonly string _accountNumber;

    public DHLCarrierService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiUrl = configuration["Carriers:DHL:ApiUrl"] ?? "https://api-eu.dhl.com/parcel/de/shipping/v2";
        _apiKey = configuration["Carriers:DHL:ApiKey"] ?? "";
        _accountNumber = configuration["Carriers:DHL:AccountNumber"] ?? "";

        _httpClient.DefaultRequestHeaders.Add("DHL-API-Key", _apiKey);
    }

    public bool SupportsCarrier(CarrierType carrier)
    {
        return carrier == CarrierType.DHL;
    }

    public async Task<ShippingLabelResponse> GenerateLabelAsync(ShippingLabelRequest request)
    {
        // TODO: Implémenter l'appel API réel à DHL
        // DHL utilise une API REST moderne

        await Task.Delay(500);

        var trackingNumber = GenerateTrackingNumber();
        var estimatedDeliveryDate = DateTime.UtcNow.AddDays(2);

        return new ShippingLabelResponse(
            TrackingNumber: trackingNumber,
            LabelUrl: $"https://example.com/labels/dhl_{trackingNumber}.pdf",
            Cost: CalculateShippingCost(request),
            EstimatedDeliveryDate: estimatedDeliveryDate
        );
    }

    public async Task<TrackingInfoDto> GetTrackingInfoAsync(string trackingNumber)
    {
        await Task.Delay(300);

        return new TrackingInfoDto(
            TrackingNumber: trackingNumber,
            CarrierName: "DHL",
            ShippedAt: DateTime.UtcNow,
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(2),
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
        // Format DHL : 10 chiffres
        var random = new Random();
        return string.Join("", Enumerable.Range(0, 10).Select(_ => random.Next(0, 10)));
    }

    private decimal CalculateShippingCost(ShippingLabelRequest request)
    {
        // DHL est un service premium
        if (request.Weight <= 0.5m) return 15.90m;
        if (request.Weight <= 1.0m) return 18.90m;
        if (request.Weight <= 2.0m) return 21.90m;
        if (request.Weight <= 5.0m) return 27.90m;
        if (request.Weight <= 10.0m) return 35.90m;
        return 49.90m;
    }
}
