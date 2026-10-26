using System.ComponentModel.DataAnnotations;

namespace Subscription.Web.Models;

public sealed class SubscriptionForm
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "100000")]
    public decimal Price { get; set; }

    [EnumDataType(typeof(BillingPeriod))]
    public BillingPeriod BillingPeriod { get; set; }

    public DateOnly? NextBillingDate { get; set; }

    [StringLength(2048)]
    [Url]
    public string? CancellationUrl { get; set; }
}
