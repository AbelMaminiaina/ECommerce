using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Services.Carriers;

/// <summary>
/// Service pour l'intégration avec Mondial Relay
/// Documentation API : https://www.mondialrelay.fr/media/108958/solutions_marchand_suivi_webservice_v16.pdf
/// </summary>
public class MondialRelayCarrierService : ICarrierService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;
    private readonly string _brandCode;
    private readonly string _apiKey;

    public MondialRelayCarrierService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiUrl = configuration["Carriers:MondialRelay:ApiUrl"] ?? "https://api.mondialrelay.com/Web_Services.asmx";
        _brandCode = configuration["Carriers:MondialRelay:BrandCode"] ?? "";
        _apiKey = configuration["Carriers:MondialRelay:ApiKey"] ?? "";
    }

    public bool SupportsCarrier(CarrierType carrier)
    {
        return carrier == CarrierType.MondialRelay;
    }

    public async Task<ShippingLabelResponse> GenerateLabelAsync(ShippingLabelRequest request)
    {
        // TODO: Implémenter l'appel API réel à Mondial Relay
        // Mondial Relay nécessite un point relais pour la livraison

        if (string.IsNullOrEmpty(request.PickupPointId))
        {
            throw new Exception("Un point relais est requis pour Mondial Relay");
        }

        await Task.Delay(500);

        var trackingNumber = GenerateTrackingNumber();
        var estimatedDeliveryDate = DateTime.UtcNow.AddDays(3);

        return new ShippingLabelResponse(
            TrackingNumber: trackingNumber,
            LabelUrl: $"https://example.com/labels/mondialrelay_{trackingNumber}.pdf",
            Cost: CalculateShippingCost(request),
            EstimatedDeliveryDate: estimatedDeliveryDate
        );
    }

    public async Task<TrackingInfoDto> GetTrackingInfoAsync(string trackingNumber)
    {
        await Task.Delay(300);

        return new TrackingInfoDto(
            TrackingNumber: trackingNumber,
            CarrierName: "Mondial Relay",
            ShippedAt: DateTime.UtcNow,
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(3),
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
        // Format Mondial Relay : 2 lettres + 10 chiffres
        var random = new Random();
        var numbers = string.Join("", Enumerable.Range(0, 10).Select(_ => random.Next(0, 10)));
        return $"MR{numbers}";
    }

    private decimal CalculateShippingCost(ShippingLabelRequest request)
    {
        // Mondial Relay est généralement moins cher (point relais)
        if (request.Weight <= 0.5m) return 3.99m;
        if (request.Weight <= 1.0m) return 4.99m;
        if (request.Weight <= 2.0m) return 5.99m;
        if (request.Weight <= 5.0m) return 7.99m;
        if (request.Weight <= 10.0m) return 11.99m;
        return 15.99m;
    }
}
