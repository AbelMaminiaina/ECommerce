using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(string id);
    Task<UserDto> UpdateUserAsync(string id, UpdateUserDto dto);
    Task<UserDto> UpdateUserRoleAsync(string id, string role);
    Task<UserDto> UpdateUserStatusAsync(string id, bool isActive);
    Task<bool> DeleteUserAsync(string id, string currentUserId);
    Task<List<UserDto>> SearchUsersAsync(string term);
    Task<UserStatsDto> GetUserStatsAsync();
}
