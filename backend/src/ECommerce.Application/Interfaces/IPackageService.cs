using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces;

public interface IPackageService
{
    Task<PackageDto> CreatePackageAsync(CreatePackageDto dto, string userId);
    Task<PackageDto> GetPackageByIdAsync(string id);
    Task<PackageDto?> GetPackageByOrderIdAsync(string orderId);
    Task<List<PackageDto>> GetAllPackagesAsync();
    Task<List<PackageDto>> GetPendingPackagesAsync();
    Task<List<PackageDto>> GetReadyToShipPackagesAsync();
    Task<PackageDto> UpdatePackageAsync(string id, UpdatePackageDto dto);
    Task<PackageDto> MarkAsPreparingAsync(string id, string adminId);
    Task<GenerateLabelResponse> GenerateLabelAsync(string id);
    Task<PackageDto> MarkAsShippedAsync(string id);
    Task DeletePackageAsync(string id);
    Task<byte[]> GenerateLabelPdfAsync(string id);
}
