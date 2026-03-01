using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamplePaymentsForOrders.Models;

namespace SamplePaymentsForOrders.Configurations;

public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(b => b.Id)
            .IsRequired();
        
        builder.HasKey(b => b.Id);
        builder.Property(b => b.UpdatedAt);
        
        builder.Property(b => b.CreatedAt)
            .IsRequired();
    }
}