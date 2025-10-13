using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    string GenerateAccessToken(UserDto user);
    string GenerateRefreshToken();
}
