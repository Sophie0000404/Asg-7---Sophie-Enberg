namespace Subscription.Api.Models;

public sealed record MonthlySummaryResponse(
    decimal EstimatedMonthlyTotal,
    int ActiveSubscriptionCount);
