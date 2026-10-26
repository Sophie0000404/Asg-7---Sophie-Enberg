# Subscription Tracker

A beginner-friendly, two-service subscription tracker built with .NET 10, ASP.NET Core Razor Pages, ASP.NET Core Web API, EF Core, SQLite, and Docker Compose.

## Architecture

```text
Browser
   |
   v
Subscription.Web (Razor Pages, port 5000)
   | HTTP
   v
Subscription.Api (ASP.NET Core API, port 5001)
   |
   v
SQLite (persistent Docker volume)
```

- **Subscription.Web** renders the dashboard and sends HTTP requests to the API. It never opens the database.
- **Subscription.Api** validates requests, owns the subscription records, uses EF Core to persist them to SQLite, and calculates the estimated monthly total.
- Docker Compose starts both services. The API's SQLite file is stored in the named `subscription-data` volume so it survives container restarts.

The app is a single-user learning project. It has no sign-in or multi-user data separation and should not be exposed publicly with real personal data.

## Run with Docker Desktop

1. Install and start Docker Desktop.
2. Open this folder in VS Code.
3. In the VS Code terminal, run:

   ```powershell
   docker compose up --build
   ```

4. Visit [http://localhost:5000](http://localhost:5000).
5. Stop the services with `Ctrl+C`. To start again, run the same Compose command; the SQLite data remains in its volume.

To stop and remove the saved database too, run `docker compose down --volumes`. This deletes the data.

## Run locally without Docker

Open two VS Code terminals from the project root:

```powershell
dotnet run --project src\Subscription.Api
```

```powershell
dotnet run --project src\Subscription.Web
```

Open [http://localhost:5000](http://localhost:5000). The local SQLite database is created as `subscriptions.db` in the API project's working directory. The API health endpoint is [http://localhost:5001/health](http://localhost:5001/health).

## Try it out

- Add subscriptions with a name, price, monthly/yearly billing period, optional next billing date, and optional provider cancellation URL.
- The monthly estimate includes active subscriptions only. A yearly price is divided by 12; for example, `$120/year` counts as `$10/month`.
- **Open provider cancellation page** takes you to the provider's website. The app cannot cancel an account at that provider automatically.
- After completing cancellation with the provider, use **Mark cancelled** to exclude it from the monthly estimate while keeping its record.

## API routes

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/subscriptions` | List subscriptions |
| `GET` | `/api/subscriptions/{id}` | Get one subscription |
| `GET` | `/api/subscriptions/summary/monthly` | Get active count and estimated monthly total |
| `POST` | `/api/subscriptions` | Add a subscription |
| `PUT` | `/api/subscriptions/{id}` | Edit a subscription |
| `POST` | `/api/subscriptions/{id}/cancel` | Mark a subscription cancelled |
| `GET` | `/health` | Check that the API is running |

## Learning notes

- The Web app calls the API using `SubscriptionApiClient` and `HttpClient`.
- The API's `SubscriptionsController` handles HTTP routes; `SubscriptionsDbContext` configures EF Core and SQLite.
- This starter uses EF Core `EnsureCreated` to keep the first run simple. It does not apply schema migrations. For later schema changes, learn EF Core migrations before relying on existing data.
- Prices are treated as USD for this first version. Authentication, multiple currencies, provider integrations, and a separate reporting microservice are intentionally out of scope.
