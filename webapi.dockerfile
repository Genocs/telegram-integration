#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /src
COPY ["src/Genocs.TelegramIntegration.WebApi", "Genocs.TelegramIntegration.WebApi/"]
COPY ["src/Genocs.TelegramIntegration.Contracts", "Genocs.TelegramIntegration.Contracts/"]
COPY ["src/Genocs.TelegramIntegration", "Genocs.TelegramIntegration/"]
COPY ["src/Genocs.TelegramIntegration.Infrastructure", "Genocs.TelegramIntegration.Infrastructure/"]

COPY ["LICENSE", "LICENSE"]
COPY ["icon.png", "icon.png"]

WORKDIR "/src/Genocs.TelegramIntegration.WebApi"

RUN dotnet restore "Genocs.TelegramIntegration.WebApi.csproj"

RUN dotnet build "Genocs.TelegramIntegration.WebApi.csproj" -c Release -o /app/build

FROM build-env AS publish
RUN dotnet publish "Genocs.TelegramIntegration.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Genocs.TelegramIntegration.WebApi.dll"]