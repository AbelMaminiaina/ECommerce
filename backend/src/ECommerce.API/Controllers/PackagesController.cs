using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PackagesController : ControllerBase
{
    private readonly IPackageService _packageService;

    public PackagesController(IPackageService packageService)
    {
        _packageService = packageService;
    }

    /// <summary>
    /// Créer un nouveau colis pour une commande
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PackageDto>> CreatePackage([FromBody] CreatePackageDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var package = await _packageService.CreatePackageAsync(dto, userId);
            return Ok(package);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Récupérer un colis par son ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PackageDto>> GetPackage(string id)
    {
        try
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            return Ok(package);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Récupérer le colis d'une commande
    /// </summary>
    [HttpGet("order/{orderId}")]
    [Authorize]
    public async Task<ActionResult<PackageDto>> GetPackageByOrder(string orderId)
    {
        try
        {
            var package = await _packageService.GetPackageByOrderIdAsync(orderId);
            if (package == null)
            {
                return NotFound(new { message = "Aucun colis trouvé pour cette commande" });
            }
            return Ok(package);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Récupérer tous les colis
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<PackageDto>>> GetAllPackages()
    {
        var packages = await _packageService.GetAllPackagesAsync();
        return Ok(packages);
    }

    /// <summary>
    /// Récupérer tous les colis en attente de préparation
    /// </summary>
    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<PackageDto>>> GetPendingPackages()
    {
        var packages = await _packageService.GetPendingPackagesAsync();
        return Ok(packages);
    }

    /// <summary>
    /// Récupérer tous les colis prêts à expédier
    /// </summary>
    [HttpGet("ready-to-ship")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<PackageDto>>> GetReadyToShipPackages()
    {
        var packages = await _packageService.GetReadyToShipPackagesAsync();
        return Ok(packages);
    }

    /// <summary>
    /// Mettre à jour un colis
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PackageDto>> UpdatePackage(string id, [FromBody] UpdatePackageDto dto)
    {
        try
        {
            var package = await _packageService.UpdatePackageAsync(id, dto);
            return Ok(package);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Marquer un colis comme en cours de préparation
    /// </summary>
    [HttpPost("{id}/prepare")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PackageDto>> MarkAsPreparing(string id)
    {
        try
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var package = await _packageService.MarkAsPreparingAsync(id, adminId);
            return Ok(package);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Générer une étiquette d'expédition
    /// </summary>
    [HttpPost("{id}/generate-label")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GenerateLabelResponse>> GenerateLabel(string id)
    {
        try
        {
            var result = await _packageService.GenerateLabelAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Marquer un colis comme expédié (envoie la notification au client)
    /// </summary>
    [HttpPost("{id}/ship")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PackageDto>> MarkAsShipped(string id)
    {
        try
        {
            var package = await _packageService.MarkAsShippedAsync(id);
            return Ok(package);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Supprimer un colis
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeletePackage(string id)
    {
        try
        {
            await _packageService.DeletePackageAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Télécharger l'étiquette PDF d'un colis
    /// </summary>
    [HttpGet("{id}/label")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DownloadLabel(string id)
    {
        try
        {
            var labelData = await _packageService.GenerateLabelPdfAsync(id);
            return File(labelData, "application/pdf", $"etiquette_{id}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
