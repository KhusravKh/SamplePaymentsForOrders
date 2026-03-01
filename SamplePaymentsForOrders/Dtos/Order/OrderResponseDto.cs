using SamplePaymentsForOrders.Models.Enums;

namespace SamplePaymentsForOrders.Dtos.Order;

public record OrderResponseDto(Guid Id, decimal Amount, string CurrencyName, OrderStatus Status);