namespace SamplePaymentsForOrders.Dtos.Order;

public record CreateOrderRequestDto(decimal Amount, Guid CurrencyId);