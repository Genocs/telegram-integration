# .NET Core libraries 



| Badge                    | Description                    |
| ------------------------ | ------------------------------ |
| Travis CI/CD             | [![Travis](https://travis-ci.org/Genocs/genocs-library.svg?branch=master)](https://travis-ci.org/Genocs/genocs-library)       |
| Github Actions           | [![.NET](https://github.com/Genocs/genocs-library/actions/workflows/build_and_test.yml/badge.svg)](https://github.com/Genocs/genocs-library/actions/workflows/build_and_test.yml)     |
| Azure CI/CD              | work in progress |
| NuGet package version    | [![NuGet](https://img.shields.io/badge/nuget-v1.0.2-blue)](https://www.nuget.org/packages/Genocs.Core) |
| NuGet package download   | [![NuGet Downloads](https://img.shields.io/nuget/dt/Genocs.Core.svg)](https://www.nuget.org/packages/Genocs.Core) |
| Discord community        | ![Discord](https://dcbadge.vercel.app/api/shield/461057072054927361?style=flat-square)  |


----

This repo contains a set of basic libraries designed by Genocs. The libraries are built using .NET standard 2.1. The package version is hosted on [nuget](https://www.nuget.org/packages).


``` bash
# Build
dotnet build .\src

# Pack
dotnet pack .\src

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

## Docker image

``` sh
docker build -t genocs/telegram_integration-webapi:1.0.6 -t genocs/telegram_integration-webapi:latest -f .\webapi.dockerfile .

docker tag genocs/telegram_integration-webapi genocs/telegram_integration-webapi:1.0.4
docker tag genocs/telegram_integration-webapi genocs/telegram_integration-webapi:latest

docker push genocs/telegram_integration-webapi:1.0.6
docker push genocs/telegram_integration-webapi:latest
``` 



``` sh
docker-compose -f .\docker-compose.yml build
``` 