using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subscription.Api.Data;
using Subscription.Api.Models;

namespace Subscription.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SubscriptionsController(SubscriptionsDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SubscriptionResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var subscriptions = await dbContext.Subscriptions
            .AsNoTracking()
            .OrderBy(subscription => subscription.Name)
            .ToListAsync(cancellationToken);

        return Ok(subscriptions.Select(ToResponse).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SubscriptionResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var subscription = await dbContext.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return subscription is null ? NotFound() : Ok(ToResponse(subscription));
    }

    [HttpGet("summary/monthly")]
    public async Task<ActionResult<MonthlySummaryResponse>> GetMonthlySummary(
        CancellationToken cancellationToken)
    {
        var activeSubscriptions = await dbContext.Subscriptions
            .AsNoTracking()
            .Where(subscription => subscription.Status == SubscriptionStatus.Active)
            .ToListAsync(cancellationToken);

        var monthlyTotal = activeSubscriptions.Sum(GetMonthlyAmount);
        return Ok(new MonthlySummaryResponse(
            decimal.Round(monthlyTotal, 2, MidpointRounding.AwayFromZero),
            activeSubscriptions.Count));
    }

    [HttpPost]
    public async Task<ActionResult<SubscriptionResponse>> Create(
        SubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var subscription = new SubscriptionRecord
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Price = request.Price,
            BillingPeriod = request.BillingPeriod,
            NextBillingDate = request.NextBillingDate,
            CancellationUrl = request.CancellationUrl,
            Status = SubscriptionStatus.Active
        };

        dbContext.Subscriptions.Add(subscription);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = subscription.Id },
            ToResponse(subscription));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SubscriptionResponse>> Update(
        Guid id,
        SubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (subscription is null)
        {
            return NotFound();
        }

        subscription.Name = request.Name.Trim();
        subscription.Price = request.Price;
        subscription.BillingPeriod = request.BillingPeriod;
        subscription.NextBillingDate = request.NextBillingDate;
        subscription.CancellationUrl = request.CancellationUrl;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ToResponse(subscription));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<SubscriptionResponse>> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (subscription is null)
        {
            return NotFound();
        }

        subscription.Status = SubscriptionStatus.Cancelled;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ToResponse(subscription));
    }

    private static SubscriptionResponse ToResponse(SubscriptionRecord subscription) =>
        new(
            subscription.Id,
            subscription.Name,
            subscription.Price,
            subscription.BillingPeriod,
            subscription.NextBillingDate,
            subscription.CancellationUrl,
            subscription.Status,
            decimal.Round(GetMonthlyAmount(subscription), 2, MidpointRounding.AwayFromZero));

    private static decimal GetMonthlyAmount(SubscriptionRecord subscription) =>
        subscription.BillingPeriod == BillingPeriod.Yearly
            ? subscription.Price / 12m
            : subscription.Price;
}
