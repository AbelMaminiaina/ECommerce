using System.Security.Claims;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WarrantyController : ControllerBase
{
    private readonly IWarrantyClaimRepository _warrantyRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public WarrantyController(
        IWarrantyClaimRepository warrantyRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _warrantyRepository = warrantyRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    /// <summary>
    /// Soumettre une réclamation de garantie
    /// </summary>
    [HttpPost("claims")]
    public async Task<ActionResult<WarrantyClaimDto>> SubmitClaim([FromBody] SubmitWarrantyClaimDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // Vérifier que la commande existe et appartient à l'utilisateur
        var order = await _orderRepository.GetByIdAsync(dto.OrderId);
        if (order == null)
            return NotFound(new { message = "Commande introuvable" });

        if (order.UserId != userId)
            return Forbid();

        // Vérifier que le produit est dans la commande
        var orderItem = order.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
        if (orderItem == null)
            return BadRequest(new { message = "Ce produit ne fait pas partie de cette commande" });

        // Récupérer les infos du produit
        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
            return NotFound(new { message = "Produit introuvable" });

        // Calculer la date d'expiration de la garantie
        var purchaseDate = order.CreatedAt;
        var warrantyExpirationDate = purchaseDate.AddMonths(product.WarrantyMonths);

        // Vérifier si la garantie est toujours valide
        if (DateTime.UtcNow > warrantyExpirationDate)
        {
            return BadRequest(new
            {
                message = $"La garantie de {product.WarrantyMonths} mois a expiré le {warrantyExpirationDate:dd/MM/yyyy}"
            });
        }

        // Vérifier qu'il n'y a pas déjà une réclamation pour ce produit
        var existingClaim = await _warrantyRepository.GetByOrderAndProductAsync(dto.OrderId, dto.ProductId);
        if (existingClaim != null)
        {
            return BadRequest(new { message = "Une réclamation existe déjà pour ce produit" });
        }

        // Créer la réclamation
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

        return Ok(MapToDto(createdClaim));
    }

    /// <summary>
    /// Obtenir toutes les réclamations de l'utilisateur connecté
    /// </summary>
    [HttpGet("claims")]
    public async Task<ActionResult<IEnumerable<WarrantyClaimDto>>> GetMyClaims()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var claims = await _warrantyRepository.GetByUserIdAsync(userId);
        var claimDtos = claims.Select(MapToDto);

        return Ok(claimDtos);
    }

    /// <summary>
    /// Obtenir une réclamation spécifique
    /// </summary>
    [HttpGet("claims/{claimId}")]
    public async Task<ActionResult<WarrantyClaimDto>> GetClaim(string claimId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var claim = await _warrantyRepository.GetByIdAsync(claimId);

        if (claim == null)
            return NotFound(new { message = "Réclamation introuvable" });

        // Vérifier les permissions
        if (claim.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        return Ok(MapToDto(claim));
    }

    /// <summary>
    /// Mettre à jour une réclamation (Admin uniquement)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("claims/{claimId}")]
    public async Task<ActionResult<WarrantyClaimDto>> UpdateClaim(
        string claimId,
        [FromBody] UpdateWarrantyClaimDto dto)
    {
        var claim = await _warrantyRepository.GetByIdAsync(claimId);

        if (claim == null)
            return NotFound();

        claim.Status = dto.Status;
        claim.Resolution = dto.Resolution;
        claim.AdminNotes = dto.AdminNotes;

        if (dto.Status == WarrantyClaimStatus.Resolved)
            claim.ResolvedAt = DateTime.UtcNow;

        claim.UpdatedAt = DateTime.UtcNow;
        await _warrantyRepository.UpdateAsync(claim);

        return Ok(MapToDto(claim));
    }

    /// <summary>
    /// Obtenir toutes les réclamations (Admin uniquement)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("claims/admin/all")]
    public async Task<ActionResult<IEnumerable<WarrantyClaimDto>>> GetAllClaims()
    {
        var claims = await _warrantyRepository.GetAllAsync();
        var claimDtos = claims.Select(MapToDto);

        return Ok(claimDtos);
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
