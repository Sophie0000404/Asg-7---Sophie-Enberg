using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.Text.Json.Serialization;
using Subscription.Api.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Subscriptions")
    ?? "Data Source=subscriptions.db";
var sqliteConnection = new SqliteConnectionStringBuilder(connectionString);
var databaseDirectory = Path.GetDirectoryName(sqliteConnection.DataSource);
if (!string.IsNullOrEmpty(databaseDirectory))
{
    Directory.CreateDirectory(databaseDirectory);
}

builder.Services.AddDbContext<SubscriptionsDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SubscriptionsDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.Run();
