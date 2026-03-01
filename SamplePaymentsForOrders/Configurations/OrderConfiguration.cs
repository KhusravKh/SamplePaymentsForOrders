using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamplePaymentsForOrders.Models;

namespace SamplePaymentsForOrders.Configurations;

public class OrderConfiguration : BaseEntityConfiguration<Order>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        base.Configure(builder);
        
        builder.Property(o => o.UserId)
            .IsRequired();
        
        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(o => o.Amount)
            .IsRequired();
        
        builder.Property(o => o.CurrencyId)
            .IsRequired();
        
        builder.HasOne(o => o.Currency)
            .WithMany()
            .HasForeignKey(o => o.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(o => o.Status)
            .IsRequired();
    }
}