using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Authentication;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace ECommerce.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly AuthService _authService;
    private readonly JwtSettings _jwtSettings;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtSettings = new JwtSettings
        {
            Secret = "ThisIsAVerySecureSecretKeyForJwtTokenGenerationWithAtLeast32Characters",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            AccessTokenExpirationMinutes = 60,
            RefreshTokenExpirationDays = 7
        };
        var jwtOptions = Options.Create(_jwtSettings);
        _authService = new AuthService(_userRepositoryMock.Object, jwtOptions);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnAuthResponse()
    {
        // Arrange
        var registerDto = new RegisterUserDto(
            "test@example.com",
            "Password123!",
            "John",
            "Doe",
            "+1234567890"
        );

        _userRepositoryMock
            .Setup(x => x.ExistsAsync(registerDto.Email))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) =>
            {
                user.Id = "test-user-id";
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                return user;
            });

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(registerDto.Email);
        result.User.FirstName.Should().Be(registerDto.FirstName);
        result.User.LastName.Should().Be(registerDto.LastName);
        result.User.Role.Should().Be("Customer");

        _userRepositoryMock.Verify(x => x.ExistsAsync(registerDto.Email), Times.Once);
        _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldThrowException()
    {
        // Arrange
        var registerDto = new RegisterUserDto(
            "existing@example.com",
            "Password123!",
            "John",
            "Doe",
            "+1234567890"
        );

        _userRepositoryMock
            .Setup(x => x.ExistsAsync(registerDto.Email))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _authService.RegisterAsync(registerDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("User with this email already exists");

        _userRepositoryMock.Verify(x => x.ExistsAsync(registerDto.Email), Times.Once);
        _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var loginDto = new LoginDto("test@example.com", "Password123!");
        var user = new User
        {
            Id = "test-user-id",
            Email = loginDto.Email,
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginDto.Password),
            PhoneNumber = "+1234567890",
            Role = "Customer",
            IsActive = true
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(user.Email);

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(loginDto.Email), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ShouldThrowException()
    {
        // Arrange
        var loginDto = new LoginDto("nonexistent@example.com", "Password123!");

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Invalid email or password");

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(loginDto.Email), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowException()
    {
        // Arrange
        var loginDto = new LoginDto("test@example.com", "WrongPassword!");
        var user = new User
        {
            Email = loginDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            IsActive = true
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ShouldThrowException()
    {
        // Arrange
        var loginDto = new LoginDto("test@example.com", "Password123!");
        var user = new User
        {
            Email = loginDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginDto.Password),
            IsActive = false
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("User account is inactive");
    }

    [Fact]
    public async Task RefreshTokenAsync_WithValidToken_ShouldReturnNewTokens()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";
        var user = new User
        {
            Id = "test-user-id",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890",
            Role = "Customer",
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
        };

        _userRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBe(refreshToken); // Should generate new token
        result.User.Email.Should().Be(user.Email);

        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var refreshToken = "invalid-refresh-token";

        _userRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());

        // Act
        Func<Task> act = async () => await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Invalid or expired refresh token");
    }

    [Fact]
    public async Task RefreshTokenAsync_WithExpiredToken_ShouldThrowException()
    {
        // Arrange
        var refreshToken = "expired-refresh-token";
        var user = new User
        {
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1) // Expired
        };

        _userRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });

        // Act
        Func<Task> act = async () => await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Invalid or expired refresh token");
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnValidJwtToken()
    {
        // Arrange
        var userDto = new UserDto(
            "test-user-id",
            "test@example.com",
            "John",
            "Doe",
            "+1234567890",
            new List<AddressDto>(),
            "Customer"
        );

        // Act
        var token = _authService.GenerateAccessToken(userDto);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT has 3 parts
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnBase64String()
    {
        // Act
        var token1 = _authService.GenerateRefreshToken();
        var token2 = _authService.GenerateRefreshToken();

        // Assert
        token1.Should().NotBeNullOrEmpty();
        token2.Should().NotBeNullOrEmpty();
        token1.Should().NotBe(token2); // Each call should generate unique token

        // Verify it's valid base64
        var bytes1 = Convert.FromBase64String(token1);
        var bytes2 = Convert.FromBase64String(token2);
        bytes1.Should().HaveCount(32);
        bytes2.Should().HaveCount(32);
    }
}
