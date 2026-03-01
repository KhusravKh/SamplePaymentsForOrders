using SamplePaymentsForOrders.Services.Abstractions;

namespace SamplePaymentsForOrders.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}