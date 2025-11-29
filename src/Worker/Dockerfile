#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /src

# Copy build configuration files first for better layer caching
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Build.targets", "Directory.Build.targets"]
COPY ["dotnet.ruleset", "dotnet.ruleset"]
COPY ["global.json", "global.json"]
COPY ["stylecop.json", "stylecop.json"]

# Copy project files only for restore (better caching)
COPY ["src/Worker/*.csproj", "Worker/"]
COPY ["src/Contracts/*.csproj", "Contracts/"]
COPY ["src/TelegramIntegration/*.csproj", "TelegramIntegration/"]
COPY ["src/Infrastructure/*.csproj", "Infrastructure/"]

WORKDIR "/src/Worker"
RUN dotnet restore "Host.csproj"

# Copy remaining source files after restore
WORKDIR /src
COPY ["src/Worker", "Worker/"]
COPY ["src/Contracts", "Contracts/"]
COPY ["src/TelegramIntegration", "TelegramIntegration/"]
COPY ["src/Infrastructure", "Infrastructure/"]
COPY ["LICENSE", "LICENSE"]
COPY ["icon.png", "icon.png"]

# Build and publish in single stage (eliminates unnecessary build stage)
WORKDIR "/src/Worker"
RUN dotnet publish "Host.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "Genocs.TelegramIntegration.Worker.dll"]