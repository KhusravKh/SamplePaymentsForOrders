using SamplePaymentsForOrders.Models.Enums;

namespace SamplePaymentsForOrders.Models;

public class Order : BaseEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; }

    public decimal Amount { get; set; }

    public Guid CurrencyId { get; set; }
    
    public Currency Currency { get; set; }
    
    public OrderStatus Status { get; set; }
}