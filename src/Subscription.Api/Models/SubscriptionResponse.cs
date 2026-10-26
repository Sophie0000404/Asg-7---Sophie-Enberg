namespace Subscription.Api.Models;

public sealed record SubscriptionResponse(
    Guid Id,
    string Name,
    decimal Price,
    BillingPeriod BillingPeriod,
    DateOnly? NextBillingDate,
    string? CancellationUrl,
    SubscriptionStatus Status,
    decimal MonthlyEquivalent);
