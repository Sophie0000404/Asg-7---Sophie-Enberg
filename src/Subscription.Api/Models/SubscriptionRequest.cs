using System.ComponentModel.DataAnnotations;

namespace Subscription.Api.Models;

public sealed class SubscriptionRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [Range(typeof(decimal), "0.01", "100000")]
    public decimal Price { get; init; }

    [EnumDataType(typeof(BillingPeriod))]
    public BillingPeriod BillingPeriod { get; init; }

    public DateOnly? NextBillingDate { get; init; }

    [StringLength(2048)]
    [Url]
    public string? CancellationUrl { get; init; }
}
