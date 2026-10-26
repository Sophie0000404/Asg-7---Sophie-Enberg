using Subscription.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<SubscriptionApiClient>(client =>
{
    var apiBaseUrl = builder.Configuration["SubscriptionApi:BaseUrl"]
        ?? "http://localhost:5001";
    client.BaseAddress = new Uri(apiBaseUrl, UriKind.Absolute);
});
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
