using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services;

public class PackageService : IPackageService
{
    private readonly IPackageRepository _packageRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICarrierService _carrierService;
    private readonly IEmailService _emailService;
    private readonly ILabelGenerator _labelGenerator;

    public PackageService(
        IPackageRepository packageRepository,
        IOrderRepository orderRepository,
        ICarrierService carrierService,
        IEmailService emailService,
        ILabelGenerator labelGenerator)
    {
        _packageRepository = packageRepository;
        _orderRepository = orderRepository;
        _carrierService = carrierService;
        _emailService = emailService;
        _labelGenerator = labelGenerator;
    }

    public async Task<PackageDto> CreatePackageAsync(CreatePackageDto dto, string userId)
    {
        // Récupérer la commande
        var order = await _orderRepository.GetByIdAsync(dto.OrderId)
            ?? throw new Exception("Commande introuvable");

        // Vérifier qu'un colis n'existe pas déjà pour cette commande
        var existingPackage = await _packageRepository.GetByOrderIdAsync(dto.OrderId);
        if (existingPackage != null)
        {
            throw new Exception("Un colis existe déjà pour cette commande");
        }

        // Créer le colis
        var package = new Package
        {
            OrderId = dto.OrderId,
            UserId = order.UserId,
            Weight = dto.Weight,
            Length = dto.Length,
            Width = dto.Width,
            Height = dto.Height,
            Carrier = dto.Carrier,
            PickupPointId = dto.PickupPointId,
            ShippingAddress = order.ShippingAddress,
            Notes = dto.Notes,
            Status = PackageStatus.Pending
        };

        var created = await _packageRepository.CreateAsync(package);
        return MapToDto(created);
    }

    public async Task<PackageDto> GetPackageByIdAsync(string id)
    {
        var package = await _packageRepository.GetByIdAsync(id)
            ?? throw new Exception("Colis introuvable");
        return MapToDto(package);
    }

    public async Task<PackageDto?> GetPackageByOrderIdAsync(string orderId)
    {
        var package = await _packageRepository.GetByOrderIdAsync(orderId);
        return package != null ? MapToDto(package) : null;
    }

    public async Task<List<PackageDto>> GetAllPackagesAsync()
    {
        var packages = await _packageRepository.GetAllAsync();
        return packages.Select(MapToDto).ToList();
    }

    public async Task<List<PackageDto>> GetPendingPackagesAsync()
    {
        var packages = await _packageRepository.GetPendingPackagesAsync();
        return packages.Select(MapToDto).ToList();
    }

    public async Task<List<PackageDto>> GetReadyToShipPackagesAsync()
    {
        var packages = await _packageRepository.GetReadyToShipPackagesAsync();
        return packages.Select(MapToDto).ToList();
    }

    public async Task<PackageDto> UpdatePackageAsync(string id, UpdatePackageDto dto)
    {
        var package = await _packageRepository.GetByIdAsync(id)
            ?? throw new Exception("Colis introuvable");

        if (dto.Weight.HasValue) package.Weight = dto.Weight.Value;
        if (dto.Length.HasValue) package.Length = dto.Length.Value;
        if (dto.Width.HasValue) package.Width = dto.Width.Value;
        if (dto.Height.HasValue) package.Height = dto.Height.Value;
        if (dto.Carrier.HasValue) package.Carrier = dto.Carrier.Value;
        if (dto.Status.HasValue) package.Status = dto.Status.Value;
        if (dto.Notes != null) package.Notes = dto.Notes;

        var updated = await _packageRepository.UpdateAsync(package);
        return MapToDto(updated);
    }

    public async Task<PackageDto> MarkAsPreparingAsync(string id, string adminId)
    {
        var package = await _packageRepository.GetByIdAsync(id)
            ?? throw new Exception("Colis introuvable");

        package.Status = PackageStatus.Preparing;
        package.PreparedBy = adminId;

        var updated = await _packageRepository.UpdateAsync(package);
        return MapToDto(updated);
    }

    public async Task<GenerateLabelResponse> GenerateLabelAsync(string id)
    {
        var package = await _packageRepository.GetByIdAsync(id)
            ?? throw new Exception("Colis introuvable");

        if (package.Status == PackageStatus.ReadyToShip)
        {
            throw new Exception("L'étiquette a déjà été générée pour ce colis");
        }

        // Créer la demande d'étiquette
        var labelRequest = new ShippingLabelRequest(
            Carrier: package.Carrier,
            FromAddress: GetWarehouseAddress(), // Adresse de l'entrepôt
            ToAddress: package.ShippingAddress,
            Weight: package.Weight,
            Length: package.Length,
            Width: package.Width,
            Height: package.Height,
            PickupPointId: package.PickupPointId,
            Reference: package.OrderId
        );

        // Générer l'étiquette via le transporteur
        var labelResponse = await _carrierService.GenerateLabelAsync(labelRequest);

        // Mettre à jour le colis
        package.TrackingNumber = labelResponse.TrackingNumber;
        package.LabelUrl = labelResponse.LabelUrl;
        package.Status = PackageStatus.ReadyToShip;
        package.PreparedAt = DateTime.UtcNow;

        await _packageRepository.UpdateAsync(package);

        // Mettre à jour la commande avec le numéro de suivi
        var order = await _orderRepository.GetByIdAsync(package.OrderId);
        if (order != null)
        {
            order.TrackingNumber = labelResponse.TrackingNumber;
            order.CarrierName = package.Carrier.ToString();
            order.EstimatedDeliveryDate = labelResponse.EstimatedDeliveryDate;
            await _orderRepository.UpdateAsync(order);
        }

        return new GenerateLabelResponse(
            labelResponse.TrackingNumber,
            labelResponse.LabelUrl,
            labelResponse.EstimatedDeliveryDate
        );
    }

    public async Task<PackageDto> MarkAsShippedAsync(string id)
    {
        var package = await _packageRepository.GetByIdAsync(id)
            ?? throw new Exception("Colis introuvable");

        if (package.Status != PackageStatus.ReadyToShip)
        {
            throw new Exception("Le colis doit être prêt à expédier avant d'être marqué comme expédié");
        }

        if (string.IsNullOrEmpty(package.TrackingNumber))
        {
            throw new Exception("Le colis doit avoir un numéro de suivi");
        }

        // Marquer comme expédié
        package.Status = PackageStatus.Shipped;
        package.ShippedAt = DateTime.UtcNow;
        await _packageRepository.UpdateAsync(package);

        // Mettre à jour la commande
        var order = await _orderRepository.GetByIdAsync(package.OrderId);
        if (order != null)
        {
            order.Status = OrderStatus.Shipped;
            order.ShippedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);
        }

        // Envoyer la notification avec le numéro de suivi
        await SendTrackingNotificationAsync(package);

        return MapToDto(package);
    }

    public async Task DeletePackageAsync(string id)
    {
        await _packageRepository.DeleteAsync(id);
    }

    public async Task<byte[]> GenerateLabelPdfAsync(string id)
    {
        var package = await _packageRepository.GetByIdAsync(id)
            ?? throw new Exception("Colis introuvable");

        if (string.IsNullOrEmpty(package.TrackingNumber))
        {
            throw new Exception("Le colis doit avoir un numéro de suivi. Générez d'abord l'étiquette.");
        }

        var order = await _orderRepository.GetByIdAsync(package.OrderId)
            ?? throw new Exception("Commande introuvable");

        // Générer le PDF avec le générateur d'étiquettes injecté
        var pdfBytes = _labelGenerator.GenerateLabel(
            package.TrackingNumber,
            package.Carrier,
            GetWarehouseAddress(),
            package.ShippingAddress,
            package.Weight,
            package.OrderId
        );

        return pdfBytes;
    }

    private async Task SendTrackingNotificationAsync(Package package)
    {
        if (package.TrackingNotificationSent)
        {
            return;
        }

        var order = await _orderRepository.GetByIdAsync(package.OrderId);
        if (order == null) return;

        // Récupérer les informations de suivi
        var trackingInfo = await _carrierService.GetTrackingInfoAsync(package.TrackingNumber!);

        // Envoyer l'email de notification
        var subject = $"Votre commande #{package.OrderId} a été expédiée !";
        var body = $@"
            <h2>Votre commande a été expédiée</h2>
            <p>Bonjour,</p>
            <p>Votre commande <strong>#{package.OrderId}</strong> a été expédiée et est en cours d'acheminement.</p>

            <h3>Informations de livraison :</h3>
            <ul>
                <li><strong>Transporteur :</strong> {package.Carrier}</li>
                <li><strong>Numéro de suivi :</strong> {package.TrackingNumber}</li>
                <li><strong>Date d'expédition :</strong> {package.ShippedAt:dd/MM/yyyy HH:mm}</li>
                <li><strong>Date de livraison estimée :</strong> {trackingInfo.EstimatedDeliveryDate:dd/MM/yyyy}</li>
            </ul>

            <p>Vous pouvez suivre votre colis en temps réel avec le numéro de suivi.</p>

            {GetCarrierTrackingLink(package.Carrier, package.TrackingNumber!)}

            <p>Merci de votre confiance !</p>
        ";

        await _emailService.SendEmailAsync(order.UserId, subject, body);

        // Marquer la notification comme envoyée
        package.TrackingNotificationSent = true;
        package.TrackingNotificationSentAt = DateTime.UtcNow;
        await _packageRepository.UpdateAsync(package);
    }

    private string GetCarrierTrackingLink(CarrierType carrier, string trackingNumber)
    {
        var link = carrier switch
        {
            CarrierType.LaPoste => $"https://www.laposte.fr/outils/suivre-vos-envois?code={trackingNumber}",
            CarrierType.Colissimo => $"https://www.laposte.fr/outils/suivre-vos-envois?code={trackingNumber}",
            CarrierType.Chronopost => $"https://www.chronopost.fr/tracking-no-cms/suivi-page?listeNumerosLT={trackingNumber}",
            CarrierType.MondialRelay => $"https://www.mondialrelay.fr/suivi-de-colis/?numeroExpedition={trackingNumber}",
            CarrierType.DHL => $"https://www.dhl.com/fr-fr/home/tracking/tracking-express.html?submit=1&tracking-id={trackingNumber}",
            CarrierType.UPS => $"https://www.ups.com/track?tracknum={trackingNumber}",
            CarrierType.FedEx => $"https://www.fedex.com/fedextrack/?tracknumbers={trackingNumber}",
            _ => ""
        };

        return string.IsNullOrEmpty(link)
            ? ""
            : $"<p><a href=\"{link}\" style=\"background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;\">Suivre mon colis</a></p>";
    }

    private Address GetWarehouseAddress()
    {
        // TODO: À configurer selon votre entrepôt
        return new Address
        {
            Street = "123 Rue de l'Entrepôt",
            City = "Paris",
            ZipCode = "75001",
            Country = "France"
        };
    }

    private PackageDto MapToDto(Package package)
    {
        return new PackageDto(
            package.Id,
            package.OrderId,
            package.Weight,
            package.Length,
            package.Width,
            package.Height,
            package.Status,
            package.Carrier,
            package.TrackingNumber,
            package.LabelUrl,
            package.PreparedAt,
            package.ShippedAt,
            package.ShippingAddress,
            package.PickupPointName,
            package.TrackingNotificationSent,
            package.Notes,
            package.CreatedAt
        );
    }
}
