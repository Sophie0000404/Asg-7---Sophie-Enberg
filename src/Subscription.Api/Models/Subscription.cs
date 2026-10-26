namespace Subscription.Api.Models;

public sealed class SubscriptionRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public BillingPeriod BillingPeriod { get; set; }
    public DateOnly? NextBillingDate { get; set; }
    public string? CancellationUrl { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
}
