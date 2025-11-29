#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /src
COPY ["src/Worker", "Worker/"]
COPY ["src/Contracts", "Contracts/"]
COPY ["src/TelegramIntegration", "TelegramIntegration/"]
COPY ["src/Infrastructure", "Infrastructure/"]

COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Build.targets", "Directory.Build.targets"]
COPY ["dotnet.ruleset", "dotnet.ruleset"]
COPY ["global.json", "global.json"]
COPY ["stylecop.json", "stylecop.json"]
COPY ["LICENSE", "LICENSE"]
COPY ["icon.png", "icon.png"]

WORKDIR "/src/Worker"

RUN dotnet restore "Host.csproj"

RUN dotnet build "Host.csproj" -c Release -o /app/build

FROM build-env AS publish
RUN dotnet publish "Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Genocs.TelegramIntegration.Worker.dll"]