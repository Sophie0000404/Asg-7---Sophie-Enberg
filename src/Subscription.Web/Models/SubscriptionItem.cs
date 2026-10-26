namespace Subscription.Web.Models;

public sealed record SubscriptionItem(
    Guid Id,
    string Name,
    decimal Price,
    BillingPeriod BillingPeriod,
    DateOnly? NextBillingDate,
    string? CancellationUrl,
    SubscriptionStatus Status,
    decimal MonthlyEquivalent);
