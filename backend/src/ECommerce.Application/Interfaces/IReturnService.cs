using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces;

public interface IReturnService
{
    Task<ReturnResponseDto> RequestReturnAsync(string orderId, string userId, RequestReturnDto dto);
    Task<ReturnResponseDto> GetReturnInfoAsync(string orderId, string userId, bool isAdmin);
    Task<ReturnResponseDto> UpdateReturnStatusAsync(string orderId, UpdateReturnStatusDto dto);
    Task<List<OrderDto>> GetAllReturnsAsync();
}
