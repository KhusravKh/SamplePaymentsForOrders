using SamplePaymentsForOrders.Dtos.Order;

namespace SamplePaymentsForOrders.Services.Abstractions;

public interface IOrderService
{
    Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderRequestDto createOrderRequestDto, CancellationToken cancellationToken);
    
    Task<OrderResponseDto> GetOrderAsync(Guid orderId, CancellationToken cancellationToken);
    
    Task<List<OrderResponseDto>> GetOrdersAsync(CancellationToken cancellationToken);
}