namespace SamplePaymentsForOrders.Services.Abstractions;

public interface IAuthenticatedService
{
    public Guid UserId { get; }
}