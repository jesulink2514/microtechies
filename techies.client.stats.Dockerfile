FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["techies.client.stats.api/Techies.Client.Stats.Api.csproj", "techies.client.stats.api/"]
RUN dotnet restore "techies.client.stats.api/Techies.Client.Stats.Api.csproj"
COPY . .
WORKDIR "/src/techies.client.stats.api"
RUN dotnet build "Techies.Client.Stats.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Techies.Client.Stats.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Techies.Client.Stats.Api.dll"]