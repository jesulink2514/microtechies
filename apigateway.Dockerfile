FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["techies.apigateway/Techies.Apigateway.csproj", "techies.apigateway/"]
RUN dotnet restore "techies.apigateway/Techies.Apigateway.csproj"
COPY . .
WORKDIR "/src/techies.apigateway"
RUN dotnet build "Techies.Apigateway.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Techies.Apigateway.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Techies.Apigateway.dll"]