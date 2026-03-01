using Microsoft.EntityFrameworkCore;
using SamplePaymentsForOrders.Models;
using SamplePaymentsForOrders.Models.Enums;
using SamplePaymentsForOrders.Services.Abstractions;

namespace SamplePaymentsForOrders.Contexts;

public class ApplicationDbContext(
    DbContextOptions options,
    IDateTimeService dateTimeService) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<OtpCode> OtpCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var passwordHash = Convert.ToBase64String("user"u8.ToArray());
        modelBuilder.Entity<User>().HasData(
            new User { FullName = "User User", PhoneNumber = "778 787 787", 
                Login = "user", PasswordHash = passwordHash, Role = UserRoles.User }
            );
    }
    
    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditInfo();
        return base.SaveChanges();
    }

    private void ApplyAuditInfo()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = dateTimeService.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = dateTimeService.UtcNow;
                    break;
            }
        }
    }
}