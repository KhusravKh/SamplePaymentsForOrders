namespace SamplePaymentsForOrders.Services.Abstractions;

public interface IDateTimeService
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}