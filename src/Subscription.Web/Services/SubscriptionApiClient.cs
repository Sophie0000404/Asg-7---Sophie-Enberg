using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Subscription.Web.Models;

namespace Subscription.Web.Services;

public sealed class SubscriptionApiClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<IReadOnlyList<SubscriptionItem>> GetSubscriptionsAsync()
    {
        var subscriptions = await httpClient.GetFromJsonAsync<List<SubscriptionItem>>(
            "api/subscriptions",
            JsonOptions);
        return subscriptions
            ?? throw new InvalidOperationException("The Subscription API returned an empty response.");
    }

    public async Task<MonthlySummary> GetMonthlySummaryAsync()
    {
        return await httpClient.GetFromJsonAsync<MonthlySummary>(
                "api/subscriptions/summary/monthly",
                JsonOptions)
            ?? throw new InvalidOperationException("The Subscription API returned an empty summary.");
    }

    public async Task CreateSubscriptionAsync(SubscriptionForm form)
    {
        using var response = await httpClient.PostAsJsonAsync("api/subscriptions", form, JsonOptions);
        response.EnsureSuccessStatusCode();
    }

    public async Task CancelSubscriptionAsync(Guid id)
    {
        using var response = await httpClient.PostAsync($"api/subscriptions/{id}/cancel", null);
        response.EnsureSuccessStatusCode();
    }
}
