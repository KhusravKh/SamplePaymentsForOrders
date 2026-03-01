using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamplePaymentsForOrders.Models;

namespace SamplePaymentsForOrders.Configurations;

public class PaymentConfiguration : BaseEntityConfiguration<Payment>
{
    public override void Configure(EntityTypeBuilder<Payment> builder)
    {
        base.Configure(builder);
        
        builder.Property(p => p.UserId)
            .IsRequired();
        
        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Order)
            .WithMany()
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        
        builder.Property(p => p.Amount)
            .IsRequired();
        
        builder.Property(p => p.Status)
            .IsRequired();
    }
}