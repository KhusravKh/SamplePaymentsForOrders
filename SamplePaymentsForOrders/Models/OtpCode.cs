namespace SamplePaymentsForOrders.Models;

public class OtpCode : BaseEntity
{
    public string PhoneNumber { get; set; }
    public string Code { get; set; }
    public DateTime ExpiresAt { get; set; }
    public short Attempts { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime CodeSendAt { get; set; }
    public DateTime? LockEndTime { get; set; }
}