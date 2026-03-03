using SamplePaymentsForOrders.Dtos.Payment;

namespace SamplePaymentsForOrders.Services.Abstractions;

public interface IPaymentService
{
    Task<CreatePaymentResponseDto> Create(CreatePaymentRequestDto paymentRequest, int millisecondsDelay, CancellationToken cancellationToken);
    
    Task ConfirmPaymentAsync(Guid paymentId, CancellationToken cancellationToken);
    
    Task<List<PaymentResponseDto>> GetPaymentByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
}