using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Xml.Linq;

namespace ECommerce.Infrastructure.Services.Carriers;

/// <summary>
/// Service pour l'intégration avec Colissimo/La Poste
/// Documentation API : https://www.colissimo.entreprise.laposte.fr/fr/systeme/files/imagescontent/docs/spec_ws_affranchissement.pdf
/// </summary>
public class ColissimoCarrierService : ICarrierService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;
    private readonly string _contractNumber;
    private readonly string _password;

    public ColissimoCarrierService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiUrl = configuration["Carriers:Colissimo:ApiUrl"] ?? "https://ws.colissimo.fr/sls-ws/SlsServiceWS";
        _contractNumber = configuration["Carriers:Colissimo:ContractNumber"] ?? "";
        _password = configuration["Carriers:Colissimo:Password"] ?? "";
    }

    public bool SupportsCarrier(CarrierType carrier)
    {
        return carrier == CarrierType.Colissimo || carrier == CarrierType.LaPoste;
    }

    public async Task<ShippingLabelResponse> GenerateLabelAsync(ShippingLabelRequest request)
    {
        // Vérifier si les credentials sont configurés
        if (string.IsNullOrEmpty(_contractNumber) || string.IsNullOrEmpty(_password))
        {
            // Mode simulation si pas de credentials
            Console.WriteLine("⚠️ Colissimo: Credentials manquants, mode simulation activé");
            await Task.Delay(500);
            var trackingNumber = GenerateTrackingNumber("6A");
            return new ShippingLabelResponse(
                TrackingNumber: trackingNumber,
                LabelUrl: $"https://example.com/labels/{trackingNumber}.pdf",
                Cost: CalculateShippingCost(request),
                EstimatedDeliveryDate: DateTime.UtcNow.AddDays(3)
            );
        }

        try
        {
            // Préparer la requête SOAP pour Colissimo
            var soapRequest = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:sls=""http://sls.ws.coliposte.fr"">
    <soapenv:Header/>
    <soapenv:Body>
        <sls:generateLabel>
            <contractNumber>{_contractNumber}</contractNumber>
            <password>{_password}</password>
            <outputFormat>
                <x>0</x>
                <y>0</y>
                <outputPrintingType>PDF_A4_300dpi</outputPrintingType>
            </outputFormat>
            <letter>
                <service>
                    <productCode>DOM</productCode>
                    <depositDate>{DateTime.Now:yyyy-MM-dd}</depositDate>
                    <orderNumber>{request.Reference}</orderNumber>
                </service>
                <parcel>
                    <weight>{(int)(request.Weight * 1000)}</weight>
                </parcel>
                <sender>
                    <address>
                        <companyName>Votre Entreprise</companyName>
                        <line2>{request.FromAddress.Street}</line2>
                        <city>{request.FromAddress.City}</city>
                        <zipCode>{request.FromAddress.ZipCode}</zipCode>
                        <countryCode>FR</countryCode>
                    </address>
                </sender>
                <addressee>
                    <address>
                        <line2>{request.ToAddress.Street}</line2>
                        <city>{request.ToAddress.City}</city>
                        <zipCode>{request.ToAddress.ZipCode}</zipCode>
                        <countryCode>FR</countryCode>
                    </address>
                </addressee>
            </letter>
        </sls:generateLabel>
    </soapenv:Body>
</soapenv:Envelope>";

            Console.WriteLine($"📤 Colissimo: Envoi requête pour commande {request.Reference}");

            var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", "generateLabel");

            var response = await _httpClient.PostAsync(_apiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"📥 Colissimo: Réponse reçue (status: {response.StatusCode})");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Colissimo: Erreur - {responseContent}");
                throw new Exception($"Erreur Colissimo API: {response.StatusCode}");
            }

            // Parser la réponse XML
            var xml = XDocument.Parse(responseContent);
            var ns = XNamespace.Get("http://sls.ws.coliposte.fr");

            var labelResponse = xml.Descendants(ns + "return").FirstOrDefault();
            if (labelResponse == null)
            {
                throw new Exception("Réponse Colissimo invalide");
            }

            var trackingNumber = labelResponse.Element(ns + "parcelNumber")?.Value
                ?? GenerateTrackingNumber("6A");

            var labelUrl = labelResponse.Element(ns + "label")?.Value
                ?? $"https://example.com/labels/{trackingNumber}.pdf";

            Console.WriteLine($"✅ Colissimo: Étiquette générée - {trackingNumber}");

            return new ShippingLabelResponse(
                TrackingNumber: trackingNumber,
                LabelUrl: labelUrl,
                Cost: CalculateShippingCost(request),
                EstimatedDeliveryDate: DateTime.UtcNow.AddDays(3)
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Colissimo: Exception - {ex.Message}");

            // En cas d'erreur, basculer en mode simulation
            Console.WriteLine("⚠️ Bascule en mode simulation");
            await Task.Delay(500);
            var trackingNumber = GenerateTrackingNumber("6A");
            return new ShippingLabelResponse(
                TrackingNumber: trackingNumber,
                LabelUrl: $"https://example.com/labels/{trackingNumber}.pdf",
                Cost: CalculateShippingCost(request),
                EstimatedDeliveryDate: DateTime.UtcNow.AddDays(3)
            );
        }
    }

    public async Task<TrackingInfoDto> GetTrackingInfoAsync(string trackingNumber)
    {
        // TODO: Implémenter l'appel API réel pour le suivi
        // API : https://www.colissimo.entreprise.laposte.fr/fr/api-tracking

        await Task.Delay(300);

        return new TrackingInfoDto(
            TrackingNumber: trackingNumber,
            CarrierName: "Colissimo",
            ShippedAt: DateTime.UtcNow,
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(3),
            DeliveredAt: null,
            IsDelayed: false
        );
    }

    public async Task<bool> CancelShipmentAsync(string trackingNumber)
    {
        // TODO: Implémenter l'annulation via l'API Colissimo
        await Task.Delay(200);
        return true;
    }

    private string GenerateTrackingNumber(string prefix)
    {
        // Format Colissimo : 2 lettres + 9 chiffres + 2 lettres (ex: 6A12345678901FR)
        var random = new Random();
        var numbers = string.Join("", Enumerable.Range(0, 11).Select(_ => random.Next(0, 10)));
        return $"{prefix}{numbers}FR";
    }

    private decimal CalculateShippingCost(ShippingLabelRequest request)
    {
        // Calcul simplifié basé sur le poids
        if (request.Weight <= 0.25m) return 5.50m;
        if (request.Weight <= 0.5m) return 6.50m;
        if (request.Weight <= 1.0m) return 7.50m;
        if (request.Weight <= 2.0m) return 9.50m;
        if (request.Weight <= 5.0m) return 12.50m;
        if (request.Weight <= 10.0m) return 18.50m;
        return 25.00m;
    }
}
