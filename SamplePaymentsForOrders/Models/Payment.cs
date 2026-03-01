using SamplePaymentsForOrders.Models.Enums;

namespace SamplePaymentsForOrders.Models;

public class Payment : BaseEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; }

    public decimal Amount { get; set; }

    public Guid OrderId { get; set; }

    public Order Order { get; set; }
    
    public PaymentStatus Status { get; set; }
}