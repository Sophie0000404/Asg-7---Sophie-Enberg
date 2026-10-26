namespace Subscription.Web.Models;

public sealed record MonthlySummary(
    decimal EstimatedMonthlyTotal,
    int ActiveSubscriptionCount);
