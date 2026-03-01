using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamplePaymentsForOrders.Models;

namespace SamplePaymentsForOrders.Configurations;

public class OtpCodeConfiguration : BaseEntityConfiguration<OtpCode>
{
    public override void Configure(EntityTypeBuilder<OtpCode> builder)
    {
        base.Configure(builder);
        
        builder.Property(b => b.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(o => o.Code)
            .HasMaxLength(6)
            .IsRequired();
        
        builder.Property(o => o.ExpiresAt)
            .IsRequired();
        
        builder.Property(o => o.Attempts)
            .IsRequired();

        builder.Property(o => o.IsUsed)
            .IsRequired();
        
        builder.Property(o => o.CodeSendAt)
            .IsRequired();

        builder.Property(o => o.LockEndTime);
    }
}