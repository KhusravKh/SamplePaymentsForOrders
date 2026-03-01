using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamplePaymentsForOrders.Models;

namespace SamplePaymentsForOrders.Configurations;

public class CurrencyConfiguration : BaseEntityConfiguration<Currency>
{
    public override void Configure(EntityTypeBuilder<Currency> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.IsoName)
            .HasMaxLength(10)
            .IsRequired();
        
        builder.Property(c => c.ShortName)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(c => c.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Symbol)
            .HasMaxLength(5)
            .IsRequired();
    }
}