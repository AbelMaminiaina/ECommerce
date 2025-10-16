using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces;

public interface IWarrantyService
{
    Task<WarrantyClaimDto> SubmitClaimAsync(string userId, SubmitWarrantyClaimDto dto);
    Task<List<WarrantyClaimDto>> GetMyClaimsAsync(string userId);
    Task<WarrantyClaimDto> GetClaimByIdAsync(string claimId, string userId, bool isAdmin);
    Task<WarrantyClaimDto> UpdateClaimAsync(string claimId, UpdateWarrantyClaimDto dto);
    Task<List<WarrantyClaimDto>> GetAllClaimsAsync();
}
