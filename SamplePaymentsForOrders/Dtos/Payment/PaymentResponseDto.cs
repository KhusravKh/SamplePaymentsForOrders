using SamplePaymentsForOrders.Models.Enums;

namespace SamplePaymentsForOrders.Dtos.Payment;

public record PaymentResponseDto(Guid Id, decimal Amount, PaymentStatus Status);