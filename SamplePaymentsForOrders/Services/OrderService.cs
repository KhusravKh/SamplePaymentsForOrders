using Microsoft.EntityFrameworkCore;
using SamplePaymentsForOrders.Common;
using SamplePaymentsForOrders.Contexts;
using SamplePaymentsForOrders.Dtos.Order;
using SamplePaymentsForOrders.Models;
using SamplePaymentsForOrders.Models.Enums;
using SamplePaymentsForOrders.Services.Abstractions;

namespace SamplePaymentsForOrders.Services;

public class OrderService(
    IAuthenticatedService authenticatedService,
    ApplicationDbContext dbContext) : IOrderService
{
    public async Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderRequestDto createOrderRequestDto, CancellationToken cancellationToken)
    {
        var isCurrencyExists = await dbContext.Currencies
            .AnyAsync(c => c.Id == createOrderRequestDto.CurrencyId, cancellationToken: cancellationToken);

        if (!isCurrencyExists)
        {
            throw new AppLogicException(ExceptionStatus.NotFound, "Currency not found");
        }

        var order = new Order
        {
            UserId = authenticatedService.UserId,
            Amount = createOrderRequestDto.Amount,
            CurrencyId = createOrderRequestDto.CurrencyId,
            Status = OrderStatus.Created
        };
        
        await dbContext.AddAsync(order, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateOrderResponseDto(order.Id);
    }

    public async Task<OrderResponseDto> GetOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .Where(o => o.Id == orderId && o.UserId == authenticatedService.UserId)
            .Select(o => new OrderResponseDto(o.Id, o.Amount, o.Currency.Name, o.Status))
            .FirstOrDefaultAsync(cancellationToken) ?? throw new AppLogicException(ExceptionStatus.NotFound, "Order not found");
    }

    public async Task<List<OrderResponseDto>> GetOrdersAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .Where(o => o.UserId == authenticatedService.UserId)
            .Select(o => new OrderResponseDto(o.Id, o.Amount, o.Currency.Name, o.Status))
            .ToListAsync(cancellationToken);
    }
}