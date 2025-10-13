using System.Security.Claims;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // GET: api/users
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();
        var userDtos = users.Select(MapToDto);
        return Ok(userDtos);
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(string id)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        // Les utilisateurs peuvent voir leur propre profil, les admins peuvent voir tous les profils
        if (currentUserId != id && !isAdmin)
            return Forbid();

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(MapToDto(user));
    }

    // PUT: api/users/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(string id, [FromBody] UpdateUserDto dto)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        // Les utilisateurs peuvent modifier leur propre profil, les admins peuvent modifier tous les profils
        if (currentUserId != id && !isAdmin)
            return Forbid();

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

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
        return Ok(MapToDto(updated));
    }

    // PUT: api/users/{id}/role
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/role")]
    public async Task<ActionResult<UserDto>> UpdateUserRole(string id, [FromBody] UpdateUserRoleDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        // Valider le rôle
        if (dto.Role != "Admin" && dto.Role != "Customer")
            return BadRequest(new { message = "Invalid role. Must be 'Admin' or 'Customer'" });

        user.Role = dto.Role;
        user.UpdatedAt = DateTime.UtcNow;

        var updated = await _userRepository.UpdateAsync(user);
        return Ok(MapToDto(updated));
    }

    // PUT: api/users/{id}/status
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")]
    public async Task<ActionResult<UserDto>> UpdateUserStatus(string id, [FromBody] UpdateUserStatusDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        user.IsActive = dto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        var updated = await _userRepository.UpdateAsync(user);
        return Ok(MapToDto(updated));
    }

    // DELETE: api/users/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Empêcher un admin de se supprimer lui-même
        if (currentUserId == id)
            return BadRequest(new { message = "Cannot delete your own account" });

        var result = await _userRepository.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    // GET: api/users/search?term=...
    [Authorize(Roles = "Admin")]
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsers([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return BadRequest(new { message = "Search term is required" });

        var users = await _userRepository.FindAsync(u =>
            u.Email.Contains(term) ||
            u.FirstName.Contains(term) ||
            u.LastName.Contains(term));

        var userDtos = users.Select(MapToDto);
        return Ok(userDtos);
    }

    // GET: api/users/stats
    [Authorize(Roles = "Admin")]
    [HttpGet("stats")]
    public async Task<ActionResult<object>> GetUserStats()
    {
        var allUsers = await _userRepository.GetAllAsync();
        var usersList = allUsers.ToList();

        var stats = new
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
        };

        return Ok(stats);
    }

    private static UserDto MapToDto(User user)
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
