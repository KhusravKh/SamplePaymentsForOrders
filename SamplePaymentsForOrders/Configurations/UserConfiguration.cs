using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamplePaymentsForOrders.Models;
using SamplePaymentsForOrders.Models.Enums;

namespace SamplePaymentsForOrders.Configurations;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(u => u.FullName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.HasIndex(u => u.PhoneNumber)
            .IsUnique();

        builder.Property(u => u.Login)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.HasIndex(u => u.Login)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.Role)
            .HasDefaultValue(UserRoles.User)
            .IsRequired();

        builder.Property(u => u.Attempts);
        
        builder.Property(u => u.LockEndTime);
    }
}