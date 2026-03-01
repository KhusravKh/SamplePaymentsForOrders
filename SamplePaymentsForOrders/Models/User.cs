using SamplePaymentsForOrders.Models.Enums;

namespace SamplePaymentsForOrders.Models;

public class User : BaseEntity
{
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public UserRoles Role { get; set; }
    public short Attempts { get; set; }
    public DateTime? LockEndTime { get; set; }
}