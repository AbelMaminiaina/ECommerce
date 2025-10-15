using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Services.Carriers;

/// <summary>
/// Service orchestrateur qui délègue aux différents transporteurs
/// </summary>
public class CarrierServiceFactory : ICarrierService
{
    private readonly IEnumerable<ICarrierService> _carrierServices;

    public CarrierServiceFactory(IEnumerable<ICarrierService> carrierServices)
    {
        _carrierServices = carrierServices;
    }

    public bool SupportsCarrier(CarrierType carrier)
    {
        return GetCarrierService(carrier) != null;
    }

    public async Task<ShippingLabelResponse> GenerateLabelAsync(ShippingLabelRequest request)
    {
        var service = GetCarrierService(request.Carrier)
            ?? throw new Exception($"Transporteur {request.Carrier} non supporté");

        return await service.GenerateLabelAsync(request);
    }

    public async Task<TrackingInfoDto> GetTrackingInfoAsync(string trackingNumber)
    {
        // Pour simplifier, on essaie avec chaque service
        // En production, on pourrait identifier le transporteur par le format du numéro
        foreach (var service in _carrierServices)
        {
            try
            {
                return await service.GetTrackingInfoAsync(trackingNumber);
            }
            catch
            {
                continue;
            }
        }

        throw new Exception($"Impossible de récupérer les informations de suivi pour {trackingNumber}");
    }

    public async Task<bool> CancelShipmentAsync(string trackingNumber)
    {
        // Même logique que GetTrackingInfoAsync
        foreach (var service in _carrierServices)
        {
            try
            {
                return await service.CancelShipmentAsync(trackingNumber);
            }
            catch
            {
                continue;
            }
        }

        throw new Exception($"Impossible d'annuler l'expédition {trackingNumber}");
    }

    private ICarrierService? GetCarrierService(CarrierType carrier)
    {
        return _carrierServices.FirstOrDefault(s => s.SupportsCarrier(carrier));
    }
}
