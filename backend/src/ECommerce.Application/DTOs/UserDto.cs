namespace ECommerce.Application.DTOs;

public record UserDto(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    List<AddressDto> Addresses,
    string Role
);

public record AddressDto(
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    bool IsDefault
);

public record RegisterUserDto(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber
);

public record LoginDto(
    string Email,
    string Password
);

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    UserDto User
);

public record UpdateUserDto(
    string FirstName,
    string LastName,
    string PhoneNumber,
    List<AddressDto> Addresses
);

public record UpdateUserRoleDto(
    string Role
);

public record UpdateUserStatusDto(
    bool IsActive
);

public class UserStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int Admins { get; set; }
    public int Customers { get; set; }
    public List<UserDto> RecentUsers { get; set; } = new();
}
