#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src
COPY ["src/Genocs.TelegramIntegration.Worker", "Genocs.TelegramIntegration.Worker/"]
COPY ["src/Genocs.TelegramIntegration.Contracts", "Genocs.TelegramIntegration.Contracts/"]
COPY ["src/Genocs.TelegramIntegration", "Genocs.TelegramIntegration/"]
COPY ["src/Genocs.TelegramIntegration.Infrastructure", "Genocs.TelegramIntegration.Infrastructure/"]

COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Build.targets", "Directory.Build.targets"]
COPY ["dotnet.ruleset", "dotnet.ruleset"]
COPY ["global.json", "global.json"]
COPY ["stylecop.json", "stylecop.json"]
COPY ["LICENSE", "LICENSE"]
COPY ["icon.png", "icon.png"]

WORKDIR "/src/Genocs.TelegramIntegration.Worker"

RUN dotnet restore "Genocs.TelegramIntegration.Worker.csproj"

RUN dotnet build "Genocs.TelegramIntegration.Worker.csproj" -c Release -o /app/build

FROM build-env AS publish
RUN dotnet publish "Genocs.TelegramIntegration.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Genocs.TelegramIntegration.Worker.dll"]