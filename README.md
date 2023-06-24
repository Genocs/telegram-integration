# .NET Core Telegram Integration library 



| Badge                    | Description                    |
| ------------------------ | ------------------------------ |
| Travis CI/CD             | N/A                            |
| Github Actions           | N/A                            |
| Azure CI/CD              | N/A                            |
| NuGet package version    | N/A                            |
| NuGet package download   | N/A                            |
| Discord community        | ![Discord](https://dcbadge.vercel.app/api/shield/461057072054927361?style=flat-square)  |


----

This repo contains a library to integrate Telegram. The library is designed by Genocs.
The libraries are built using .NET standard 2.1. The package version is hosted on [nuget](https://www.nuget.org/packages).


``` PS
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Run worker
dotnet run --project .\src\Genocs.TelegramIntegration.WebApi
dotnet run --project .\src\Genocs.TelegramIntegration.Worker

# ** DOCKER ** #
# Build docker image
docker build -f webapi.dockerfile -t genocs/telegram_integration-webapi:2.0.1 -t genocs/telegram_integration-webapi:latest .
docker build -f worker.dockerfile -t genocs/telegram_integration-worker:2.0.1 -t genocs/telegram_integration-worker:latest .

# Push image on dockerhub
docker push genocs/telegram_integration-webapi:2.0.1
docker push genocs/telegram_integration-webapi:latest

docker push genocs/telegram_integration-worker:2.0.1
docker push genocs/telegram_integration-worker:latest

# Push image on dockerhub
docker-compose up

```




``` PS
# Create the namespace
az servicebus namespace create --resource-group rg-genocs --name asb-genocs --location "West Europe"

# Create the queue
az servicebus queue create --resource-group rg-genocs --namespace-name asb-genocs --name queue_1

# get the connection string
az servicebus namespace authorization-rule keys list --resource-group rg-genocs --namespace-name asb-genocs --name RootManageSharedAccessKey --query primaryConnectionString --output tsv 

```


## Support

api-workbench.rest

Use this file inside Visual Studio code with [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) plugin 

