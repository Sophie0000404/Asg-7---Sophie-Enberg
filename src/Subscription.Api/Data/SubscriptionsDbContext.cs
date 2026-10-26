using Microsoft.EntityFrameworkCore;
using Subscription.Api.Models;

namespace Subscription.Api.Data;

public sealed class SubscriptionsDbContext(DbContextOptions<SubscriptionsDbContext> options)
    : DbContext(options)
{
    public DbSet<SubscriptionRecord> Subscriptions => Set<SubscriptionRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SubscriptionRecord>(entity =>
        {
            entity.HasKey(subscription => subscription.Id);
            entity.Property(subscription => subscription.Name).HasMaxLength(100).IsRequired();
            entity.Property(subscription => subscription.Price).HasPrecision(10, 2);
            entity.Property(subscription => subscription.CancellationUrl).HasMaxLength(2048);
        });
    }
}
