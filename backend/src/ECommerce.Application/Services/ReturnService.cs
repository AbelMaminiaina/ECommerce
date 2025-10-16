using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services;

public class ReturnService : IReturnService
{
    private readonly IOrderRepository _orderRepository;

    public ReturnService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<ReturnResponseDto> RequestReturnAsync(string orderId, string userId, RequestReturnDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new Exception("Commande introuvable");

        // Vérifier que la commande appartient à l'utilisateur
        if (order.UserId != userId)
            throw new UnauthorizedAccessException("Accès non autorisé à cette commande");

        // Vérifier si le retour est possible
        if (!order.CanReturn)
        {
            if (order.ReturnStatus != ReturnStatus.None)
                throw new Exception("Un retour a déjà été demandé pour cette commande");

            if (order.Status != OrderStatus.Delivered)
                throw new Exception("La commande doit être livrée pour être retournée");

            if (!order.DeliveredAt.HasValue)
                throw new Exception("La date de livraison n'est pas définie");

            if (DateTime.UtcNow > order.ReturnDeadline)
                throw new Exception($"Le délai de rétractation de 14 jours est dépassé. Date limite: {order.ReturnDeadline:dd/MM/yyyy}");

            throw new Exception("Le retour n'est pas possible pour cette commande");
        }

        // Enregistrer la demande de retour
        order.ReturnRequestedAt = DateTime.UtcNow;
        order.ReturnReason = dto.Reason;
        order.ReturnStatus = ReturnStatus.Requested;
        order.Status = OrderStatus.ReturnRequested;

        await _orderRepository.UpdateAsync(order);

        return new ReturnResponseDto(
            order.Id,
            order.ReturnStatus,
            order.ReturnReason ?? "",
            order.ReturnRequestedAt.Value,
            order.ReturnDeadline
        );
    }

    public async Task<ReturnResponseDto> GetReturnInfoAsync(string orderId, string userId, bool isAdmin)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new Exception("Commande introuvable");

        if (order.UserId != userId && !isAdmin)
            throw new UnauthorizedAccessException("Accès non autorisé à cette commande");

        if (order.ReturnStatus == ReturnStatus.None)
            throw new Exception("Aucun retour demandé pour cette commande");

        return new ReturnResponseDto(
            order.Id,
            order.ReturnStatus,
            order.ReturnReason ?? "",
            order.ReturnRequestedAt ?? DateTime.MinValue,
            order.ReturnDeadline
        );
    }

    public async Task<ReturnResponseDto> UpdateReturnStatusAsync(string orderId, UpdateReturnStatusDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new Exception("Commande introuvable");

        if (order.ReturnStatus == ReturnStatus.None)
            throw new Exception("Aucun retour n'a été demandé pour cette commande");

        // Mettre à jour le statut
        order.ReturnStatus = dto.Status;

        // Si le retour est approuvé et le produit reçu, rembourser et marquer comme retourné
        if (dto.Status == ReturnStatus.Refunded)
        {
            order.Status = OrderStatus.Returned;
            order.PaymentStatus = PaymentStatus.Refunded;
        }
        else if (dto.Status == ReturnStatus.Rejected)
        {
            // Si rejeté, remettre en "Delivered"
            order.Status = OrderStatus.Delivered;
        }

        await _orderRepository.UpdateAsync(order);

        return new ReturnResponseDto(
            order.Id,
            order.ReturnStatus,
            order.ReturnReason ?? "",
            order.ReturnRequestedAt ?? DateTime.MinValue,
            order.ReturnDeadline
        );
    }

    public async Task<List<OrderDto>> GetAllReturnsAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        var returns = orders
            .Where(o => o.ReturnStatus != ReturnStatus.None)
            .Select(MapToDto)
            .ToList();

        return returns;
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto(
            order.Id,
            order.UserId,
            order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.Quantity, i.Price)).ToList(),
            order.TotalAmount,
            order.Status,
            new AddressDto(
                order.ShippingAddress.Street,
                order.ShippingAddress.City,
                order.ShippingAddress.State,
                order.ShippingAddress.ZipCode,
                order.ShippingAddress.Country,
                order.ShippingAddress.IsDefault
            ),
            order.PaymentStatus,
            order.CreatedAt,
            order.DeliveredAt,
            order.ReturnRequestedAt,
            order.ReturnReason,
            order.ReturnStatus,
            order.ReturnDeadline,
            order.CanReturn,
            order.EstimatedDeliveryDate,
            order.ShippedAt,
            order.TrackingNumber,
            order.CarrierName,
            order.EstimatedDeliveryDays,
            order.IsDeliveryDelayed
        );
    }
}
