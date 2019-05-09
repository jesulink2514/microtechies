FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["techies.client.api/Techies.Client.Api.csproj", "techies.client.api/"]
COPY ["Techies.Clients.DTOs/Techies.Clients.DTOs.csproj", "Techies.Clients.DTOs/"]
COPY ["Techies.Clients.Infrastructure.EFCore/Techies.Clients.Infrastructure.EFCore.csproj", "Techies.Clients.Infrastructure.EFCore/"]
COPY ["Techies.Clients.Infrastructure/Techies.Clients.Infrastructure.csproj", "Techies.Clients.Infrastructure/"]
COPY ["Techies.Clients.Domain/Techies.Clients.Domain.csproj", "Techies.Clients.Domain/"]
COPY ["Techies.Clients.ApplicationServices/Techies.Clients.ApplicationServices.csproj", "Techies.Clients.ApplicationServices/"]
RUN dotnet restore "techies.client.api/Techies.Client.Api.csproj"
COPY . .
WORKDIR "/src/techies.client.api"
RUN dotnet build "Techies.Client.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Techies.Client.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Techies.Client.Api.dll"]