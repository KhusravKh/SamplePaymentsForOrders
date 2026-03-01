using System.ComponentModel.DataAnnotations;

namespace SamplePaymentsForOrders.Common;

public class JwtConfig
{
    [MinLength(32)]
    public string Key { get; set; }
    [MinLength(5)]
    public string Issuer { get; set; }
    [MinLength(5)]
    public string Audience { get; set; }
    [Range(1, 60)]
    public int ExpireInMinutes { get; set; }
}