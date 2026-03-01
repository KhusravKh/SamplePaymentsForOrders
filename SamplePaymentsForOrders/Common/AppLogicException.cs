namespace SamplePaymentsForOrders.Common;

public class AppLogicException(ExceptionStatus status, string? message) : Exception(message)
{
    public ExceptionStatus Status { get; set; } = status;
}