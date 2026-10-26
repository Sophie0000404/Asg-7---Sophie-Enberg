using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Subscription.Web.Models;
using Subscription.Web.Services;

namespace Subscription.Web.Pages;

public sealed class IndexModel(SubscriptionApiClient apiClient) : PageModel
{
    [BindProperty]
    public SubscriptionForm Input { get; set; } = new();

    public IReadOnlyList<SubscriptionItem> Subscriptions { get; private set; } = [];
    public MonthlySummary Summary { get; private set; } = new(0m, 0);
    public string? ApiError { get; private set; }
    public bool DashboardAvailable { get; private set; }

    public async Task OnGetAsync()
    {
        await LoadDashboardAsync();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDashboardAsync();
            return Page();
        }

        try
        {
            await apiClient.CreateSubscriptionAsync(Input);
            TempData["Notice"] = $"{Input.Name} was added.";
            return RedirectToPage();
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "Could not save the subscription. Check that the API is running and try again.");
            await LoadDashboardAsync();
            return Page();
        }
    }

    public async Task<IActionResult> OnPostCancelAsync(Guid id)
    {
        try
        {
            await apiClient.CancelSubscriptionAsync(id);
            TempData["Notice"] = "The subscription was marked as cancelled.";
            return RedirectToPage();
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "Could not update the subscription. Check that the API is running and try again.");
            await LoadDashboardAsync();
            return Page();
        }
    }

    private async Task LoadDashboardAsync()
    {
        try
        {
            var subscriptionsTask = apiClient.GetSubscriptionsAsync();
            var summaryTask = apiClient.GetMonthlySummaryAsync();
            await Task.WhenAll(subscriptionsTask, summaryTask);

            Subscriptions = await subscriptionsTask;
            Summary = await summaryTask;
            DashboardAvailable = true;
        }
        catch (HttpRequestException)
        {
            ApiError = "The Subscription API is unavailable. Start the API service and refresh this page.";
        }
    }
}
