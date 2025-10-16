using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services;

public class WarrantyService : IWarrantyService
{
    private readonly IWarrantyClaimRepository _warrantyRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public WarrantyService(
        IWarrantyClaimRepository warrantyRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _warrantyRepository = warrantyRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<WarrantyClaimDto> SubmitClaimAsync(string userId, SubmitWarrantyClaimDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(dto.OrderId);
        if (order == null)
            throw new Exception("Commande introuvable");

        if (order.UserId != userId)
            throw new UnauthorizedAccessException("Accès non autorisé à cette commande");

        var orderItem = order.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
        if (orderItem == null)
            throw new Exception("Ce produit ne fait pas partie de cette commande");

        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
            throw new Exception("Produit introuvable");

        var purchaseDate = order.CreatedAt;
        var warrantyExpirationDate = purchaseDate.AddMonths(product.WarrantyMonths);

        if (DateTime.UtcNow > warrantyExpirationDate)
        {
            throw new Exception($"La garantie de {product.WarrantyMonths} mois a expiré le {warrantyExpirationDate:dd/MM/yyyy}");
        }

        var existingClaim = await _warrantyRepository.GetByOrderAndProductAsync(dto.OrderId, dto.ProductId);
        if (existingClaim != null)
            throw new Exception("Une réclamation existe déjà pour ce produit");

        var claim = new WarrantyClaim
        {
            OrderId = dto.OrderId,
            ProductId = dto.ProductId,
            ProductName = orderItem.ProductName,
            UserId = userId,
            PurchaseDate = purchaseDate,
            WarrantyExpirationDate = warrantyExpirationDate,
            IssueDescription = dto.IssueDescription,
            Photos = dto.Photos ?? new List<string>()
        };

        var createdClaim = await _warrantyRepository.CreateAsync(claim);
        return MapToDto(createdClaim);
    }

    public async Task<List<WarrantyClaimDto>> GetMyClaimsAsync(string userId)
    {
        var claims = await _warrantyRepository.GetByUserIdAsync(userId);
        return claims.Select(MapToDto).ToList();
    }

    public async Task<WarrantyClaimDto> GetClaimByIdAsync(string claimId, string userId, bool isAdmin)
    {
        var claim = await _warrantyRepository.GetByIdAsync(claimId);
        if (claim == null)
            throw new Exception("Réclamation introuvable");

        if (claim.UserId != userId && !isAdmin)
            throw new UnauthorizedAccessException("Accès non autorisé à cette réclamation");

        return MapToDto(claim);
    }

    public async Task<WarrantyClaimDto> UpdateClaimAsync(string claimId, UpdateWarrantyClaimDto dto)
    {
        var claim = await _warrantyRepository.GetByIdAsync(claimId);
        if (claim == null)
            throw new Exception("Réclamation introuvable");

        claim.Status = dto.Status;
        claim.Resolution = dto.Resolution;
        claim.AdminNotes = dto.AdminNotes;

        if (dto.Status == WarrantyClaimStatus.Resolved)
            claim.ResolvedAt = DateTime.UtcNow;

        claim.UpdatedAt = DateTime.UtcNow;
        await _warrantyRepository.UpdateAsync(claim);

        return MapToDto(claim);
    }

    public async Task<List<WarrantyClaimDto>> GetAllClaimsAsync()
    {
        var claims = await _warrantyRepository.GetAllAsync();
        return claims.Select(MapToDto).ToList();
    }

    private static WarrantyClaimDto MapToDto(WarrantyClaim claim)
    {
        var isUnderWarranty = DateTime.UtcNow <= claim.WarrantyExpirationDate;

        return new WarrantyClaimDto(
            claim.Id,
            claim.OrderId,
            claim.ProductId,
            claim.ProductName,
            claim.UserId,
            claim.PurchaseDate,
            claim.WarrantyExpirationDate,
            claim.IssueDescription,
            claim.Photos,
            claim.Status,
            claim.Resolution,
            claim.AdminNotes,
            claim.CreatedAt,
            claim.ResolvedAt,
            isUnderWarranty
        );
    }
}
