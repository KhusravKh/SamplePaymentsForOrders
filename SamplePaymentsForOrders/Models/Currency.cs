namespace SamplePaymentsForOrders.Models;

public class Currency : BaseEntity
{
    public string IsoName { get; set; }

    public string ShortName { get; set; }

    public string Name { get; set; }

    public string Symbol { get; set; }
}