FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY ["src/Subscription.Web/Subscription.Web.csproj", "src/Subscription.Web/"]
RUN dotnet restore "src/Subscription.Web/Subscription.Web.csproj"
COPY . .
WORKDIR /source/src/Subscription.Web
RUN dotnet publish "Subscription.Web.csproj" --configuration Release --output /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Subscription.Web.dll"]
