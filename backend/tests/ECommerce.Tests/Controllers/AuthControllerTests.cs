using ECommerce.API.Controllers;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ECommerce.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnOkWithAuthResponse()
    {
        // Arrange
        var registerDto = new RegisterUserDto(
            "test@example.com",
            "Password123!",
            "John",
            "Doe",
            "+1234567890"
        );

        var authResponse = new AuthResponseDto(
            "access-token",
            "refresh-token",
            new UserDto(
                "user-id",
                registerDto.Email,
                registerDto.FirstName,
                registerDto.LastName,
                registerDto.PhoneNumber,
                new List<AddressDto>(),
                "Customer"
            )
        );

        _authServiceMock
            .Setup(x => x.RegisterAsync(registerDto))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAuth = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;
        returnedAuth.AccessToken.Should().Be(authResponse.AccessToken);
        returnedAuth.RefreshToken.Should().Be(authResponse.RefreshToken);
        returnedAuth.User.Email.Should().Be(registerDto.Email);

        _authServiceMock.Verify(x => x.RegisterAsync(registerDto), Times.Once);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var registerDto = new RegisterUserDto(
            "existing@example.com",
            "Password123!",
            "John",
            "Doe",
            "+1234567890"
        );

        var errorMessage = "User with this email already exists";

        _authServiceMock
            .Setup(x => x.RegisterAsync(registerDto))
            .ThrowsAsync(new Exception(errorMessage));

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorResponse = badRequestResult.Value.Should().BeEquivalentTo(new { message = errorMessage });

        _authServiceMock.Verify(x => x.RegisterAsync(registerDto), Times.Once);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkWithAuthResponse()
    {
        // Arrange
        var loginDto = new LoginDto("test@example.com", "Password123!");

        var authResponse = new AuthResponseDto(
            "access-token",
            "refresh-token",
            new UserDto(
                "user-id",
                loginDto.Email,
                "John",
                "Doe",
                "+1234567890",
                new List<AddressDto>(),
                "Customer"
            )
        );

        _authServiceMock
            .Setup(x => x.LoginAsync(loginDto))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAuth = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;
        returnedAuth.AccessToken.Should().Be(authResponse.AccessToken);
        returnedAuth.RefreshToken.Should().Be(authResponse.RefreshToken);
        returnedAuth.User.Email.Should().Be(loginDto.Email);

        _authServiceMock.Verify(x => x.LoginAsync(loginDto), Times.Once);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnBadRequest()
    {
        // Arrange
        var loginDto = new LoginDto("test@example.com", "WrongPassword!");
        var errorMessage = "Invalid email or password";

        _authServiceMock
            .Setup(x => x.LoginAsync(loginDto))
            .ThrowsAsync(new Exception(errorMessage));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorResponse = badRequestResult.Value.Should().BeEquivalentTo(new { message = errorMessage });

        _authServiceMock.Verify(x => x.LoginAsync(loginDto), Times.Once);
    }

    [Fact]
    public async Task Login_WithInactiveUser_ShouldReturnBadRequest()
    {
        // Arrange
        var loginDto = new LoginDto("inactive@example.com", "Password123!");
        var errorMessage = "User account is inactive";

        _authServiceMock
            .Setup(x => x.LoginAsync(loginDto))
            .ThrowsAsync(new Exception(errorMessage));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorResponse = badRequestResult.Value.Should().BeEquivalentTo(new { message = errorMessage });

        _authServiceMock.Verify(x => x.LoginAsync(loginDto), Times.Once);
    }

    [Fact]
    public async Task Refresh_WithValidToken_ShouldReturnOkWithNewTokens()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";

        var authResponse = new AuthResponseDto(
            "new-access-token",
            "new-refresh-token",
            new UserDto(
                "user-id",
                "test@example.com",
                "John",
                "Doe",
                "+1234567890",
                new List<AddressDto>(),
                "Customer"
            )
        );

        _authServiceMock
            .Setup(x => x.RefreshTokenAsync(refreshToken))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _controller.Refresh(refreshToken);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAuth = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;
        returnedAuth.AccessToken.Should().Be(authResponse.AccessToken);
        returnedAuth.RefreshToken.Should().Be(authResponse.RefreshToken);

        _authServiceMock.Verify(x => x.RefreshTokenAsync(refreshToken), Times.Once);
    }

    [Fact]
    public async Task Refresh_WithInvalidToken_ShouldReturnBadRequest()
    {
        // Arrange
        var refreshToken = "invalid-refresh-token";
        var errorMessage = "Invalid or expired refresh token";

        _authServiceMock
            .Setup(x => x.RefreshTokenAsync(refreshToken))
            .ThrowsAsync(new Exception(errorMessage));

        // Act
        var result = await _controller.Refresh(refreshToken);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorResponse = badRequestResult.Value.Should().BeEquivalentTo(new { message = errorMessage });

        _authServiceMock.Verify(x => x.RefreshTokenAsync(refreshToken), Times.Once);
    }

    [Fact]
    public async Task Refresh_WithExpiredToken_ShouldReturnBadRequest()
    {
        // Arrange
        var refreshToken = "expired-refresh-token";
        var errorMessage = "Invalid or expired refresh token";

        _authServiceMock
            .Setup(x => x.RefreshTokenAsync(refreshToken))
            .ThrowsAsync(new Exception(errorMessage));

        // Act
        var result = await _controller.Refresh(refreshToken);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorResponse = badRequestResult.Value.Should().BeEquivalentTo(new { message = errorMessage });

        _authServiceMock.Verify(x => x.RefreshTokenAsync(refreshToken), Times.Once);
    }
}
