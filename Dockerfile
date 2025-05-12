# Base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source
COPY ConwayGame.sln .
COPY ConwayGame.Api/ConwayGame.Api.csproj ConwayGame.Api/
COPY ConwayGame.Infrastructure/ConwayGame.Infrastructure.csproj ConwayGame.Infrastructure/
COPY ConwayGame.Tests/ConwayGame.Tests.csproj ConwayGame.Tests/
RUN dotnet restore
COPY . .
WORKDIR /source
RUN dotnet test ConwayGame.Tests/ConwayGame.Tests.csproj --no-restore --verbosity normal
RUN dotnet build ConwayGame.Api/ConwayGame.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ConwayGame.Api/ConwayGame.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Final stage with production ready image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ConwayGame.Api.dll"]