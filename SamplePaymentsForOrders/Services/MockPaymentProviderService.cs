using SamplePaymentsForOrders.Services.Abstractions;

namespace SamplePaymentsForOrders.Services;

public class MockPaymentProviderService : IMockPaymentProviderService
{
    public Task<bool> Pay(decimal amount, string currency, CancellationToken cancellationToken)
    {
        var random = new Random();
        var success = random.Next(0, 2) == 0;
        return Task.FromResult(success);
    }
}