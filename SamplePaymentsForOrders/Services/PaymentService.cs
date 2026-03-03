using Microsoft.EntityFrameworkCore;
using SamplePaymentsForOrders.Common;
using SamplePaymentsForOrders.Contexts;
using SamplePaymentsForOrders.Dtos.Payment;
using SamplePaymentsForOrders.Models;
using SamplePaymentsForOrders.Models.Enums;
using SamplePaymentsForOrders.Services.Abstractions;

namespace SamplePaymentsForOrders.Services;

public class PaymentService(
    ApplicationDbContext dbContext,
    IAuthenticatedService authenticatedService,
    IMockPaymentProviderService mockPaymentProviderService) : IPaymentService
{
    public async Task<CreatePaymentResponseDto> Create(CreatePaymentRequestDto paymentRequest, int millisecondsDelay,  CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var order = await dbContext.Orders
                        .FromSqlRaw("SELECT * FROM \"Orders\" FOR UPDATE")
                        .FirstOrDefaultAsync(o => o.Id == paymentRequest.OrderId 
                                                  && o.UserId == authenticatedService.UserId, cancellationToken)
                    ?? throw new Exception("Order not found");

        if (order.Status != OrderStatus.Created)
        {
            throw new Exception("Order is not in a valid state for payment");
        }
        
        var isAnyPayment = await dbContext.Payments
            .AnyAsync(p => p.OrderId == paymentRequest.OrderId 
                             && p.Status == PaymentStatus.Pending, cancellationToken);

        if (isAnyPayment)
        {
            throw new Exception("Payment for this order already exists");
        }
        
        var payment = new Payment
        {
            UserId = authenticatedService.UserId,
            Amount = order.Amount,
            Status = PaymentStatus.Pending,
            OrderId = order.Id
        };
        
        await dbContext.Payments.AddAsync(payment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        
        return new CreatePaymentResponseDto(payment.Id);
    }

    public async Task ConfirmPaymentAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        var payment = await dbContext.Payments
            .FromSqlRaw("SELECT * FROM \"Payments\" FOR UPDATE")
            .FirstOrDefaultAsync(p => p.Id == paymentId 
                                      && p.UserId == authenticatedService.UserId, cancellationToken) 
                      ?? throw new AppLogicException( ExceptionStatus.NotFound, "Payment not found");

        if (payment.Status != PaymentStatus.Pending)
        {
            throw new AppLogicException(ExceptionStatus.BadRequest, "Payment is not in a valid state for confirmation");
        }
        
        var order = await dbContext.Orders
                        .Include(o => o.Currency)
                        .FirstOrDefaultAsync(o => o.Id == payment.OrderId, cancellationToken)
                    ?? throw new AppLogicException(ExceptionStatus.NotFound, "Order not found");
        
        var paid = await mockPaymentProviderService.Pay(payment.Amount, order.Currency.IsoName, cancellationToken);

        if (paid)
        {
            payment.Status = PaymentStatus.Successful;
            order.Status = OrderStatus.Paid;
        }
        else
        {
            payment.Status = PaymentStatus.Failed;
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<List<PaymentResponseDto>> GetPaymentByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await dbContext.Payments
            .Where(p => p.OrderId == orderId && p.UserId == authenticatedService.UserId)
            .Select(p => new PaymentResponseDto(p.Id, p.Amount, p.Status))
            .ToListAsync(cancellationToken);
    }
}