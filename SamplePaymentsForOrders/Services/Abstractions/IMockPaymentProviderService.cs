namespace SamplePaymentsForOrders.Services.Abstractions;

public interface IMockPaymentProviderService
{
    Task<bool> Pay(decimal amount, string currency, CancellationToken cancellationToken);
}