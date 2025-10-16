using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto).ToList();
    }

    public async Task<UserDto> GetUserByIdAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new Exception("Utilisateur introuvable");

        return MapToDto(user);
    }

    public async Task<UserDto> UpdateUserAsync(string id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new Exception("Utilisateur introuvable");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;
        user.Addresses = dto.Addresses.Select(a => new Address
        {
            Street = a.Street,
            City = a.City,
            State = a.State,
            ZipCode = a.ZipCode,
            Country = a.Country,
            IsDefault = a.IsDefault
        }).ToList();
        user.UpdatedAt = DateTime.UtcNow;

        var updated = await _userRepository.UpdateAsync(user);
        return MapToDto(updated);
    }

    public async Task<UserDto> UpdateUserRoleAsync(string id, string role)
    {
        // Valider le rôle
        if (role != "Admin" && role != "Customer")
            throw new ArgumentException("Invalid role. Must be 'Admin' or 'Customer'");

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new Exception("Utilisateur introuvable");

        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;

        var updated = await _userRepository.UpdateAsync(user);
        return MapToDto(updated);
    }

    public async Task<UserDto> UpdateUserStatusAsync(string id, bool isActive)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new Exception("Utilisateur introuvable");

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;

        var updated = await _userRepository.UpdateAsync(user);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteUserAsync(string id, string currentUserId)
    {
        // Empêcher un admin de se supprimer lui-même
        if (currentUserId == id)
            throw new InvalidOperationException("Cannot delete your own account");

        return await _userRepository.DeleteAsync(id);
    }

    public async Task<List<UserDto>> SearchUsersAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            throw new ArgumentException("Search term is required");

        var users = await _userRepository.FindAsync(u =>
            u.Email.Contains(term) ||
            u.FirstName.Contains(term) ||
            u.LastName.Contains(term));

        return users.Select(MapToDto).ToList();
    }

    public async Task<UserStatsDto> GetUserStatsAsync()
    {
        var allUsers = await _userRepository.GetAllAsync();
        var usersList = allUsers.ToList();

        return new UserStatsDto
        {
            TotalUsers = usersList.Count,
            ActiveUsers = usersList.Count(u => u.IsActive),
            InactiveUsers = usersList.Count(u => !u.IsActive),
            Admins = usersList.Count(u => u.Role == "Admin"),
            Customers = usersList.Count(u => u.Role == "Customer"),
            RecentUsers = usersList
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .Select(MapToDto)
                .ToList()
        };
    }

    private static UserDto MapToDto(Domain.Entities.User user)
    {
        return new UserDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.Addresses.Select(a => new AddressDto(
                a.Street,
                a.City,
                a.State,
                a.ZipCode,
                a.Country,
                a.IsDefault
            )).ToList(),
            user.Role
        );
    }
}
