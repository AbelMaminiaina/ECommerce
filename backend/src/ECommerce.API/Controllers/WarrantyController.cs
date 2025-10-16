using System.Security.Claims;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WarrantyController : ControllerBase
{
    private readonly IWarrantyService _warrantyService;

    public WarrantyController(IWarrantyService warrantyService)
    {
        _warrantyService = warrantyService;
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

        try
        {
            var claim = await _warrantyService.SubmitClaimAsync(userId, dto);
            return Ok(claim);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
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

        var claims = await _warrantyService.GetMyClaimsAsync(userId);
        return Ok(claims);
    }

    /// <summary>
    /// Obtenir une réclamation spécifique
    /// </summary>
    [HttpGet("claims/{claimId}")]
    public async Task<ActionResult<WarrantyClaimDto>> GetClaim(string claimId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var isAdmin = User.IsInRole("Admin");
            var claim = await _warrantyService.GetClaimByIdAsync(claimId, userId, isAdmin);
            return Ok(claim);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
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
        try
        {
            var claim = await _warrantyService.UpdateClaimAsync(claimId, dto);
            return Ok(claim);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtenir toutes les réclamations (Admin uniquement)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("claims/admin/all")]
    public async Task<ActionResult<IEnumerable<WarrantyClaimDto>>> GetAllClaims()
    {
        var claims = await _warrantyService.GetAllClaimsAsync();
        return Ok(claims);
    }
}
