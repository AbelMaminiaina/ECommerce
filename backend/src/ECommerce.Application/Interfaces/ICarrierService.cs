using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces;

/// <summary>
/// Interface pour les services de transporteurs
/// </summary>
public interface ICarrierService
{
    /// <summary>
    /// Génère une étiquette d'expédition
    /// </summary>
    Task<ShippingLabelResponse> GenerateLabelAsync(ShippingLabelRequest request);

    /// <summary>
    /// Récupère les informations de suivi d'un colis
    /// </summary>
    Task<TrackingInfoDto> GetTrackingInfoAsync(string trackingNumber);

    /// <summary>
    /// Annule une expédition
    /// </summary>
    Task<bool> CancelShipmentAsync(string trackingNumber);

    /// <summary>
    /// Vérifie si ce service supporte le transporteur spécifié
    /// </summary>
    bool SupportsCarrier(CarrierType carrier);
}
