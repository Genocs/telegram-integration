<!-- PROJECT SHIELDS -->
[![License][license-shield]][license-url]
[![Build][build-shield]][build-url]
[![Packages][package-shield]][package-url]
[![Downloads][downloads-shield]][downloads-url]
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![Discord][discord-shield]][discord-url]
[![Gitter][gitter-shield]][gitter-url]
[![Twitter][twitter-shield]][twitter-url]
[![Twitterx][twitterx-shield]][twitterx-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

[license-shield]: https://img.shields.io/github/license/Genocs/telegram-integration?color=2da44e&style=flat-square
[license-url]: https://github.com/Genocs/telegram-integration/blob/main/LICENSE
[build-shield]: https://github.com/Genocs/telegram-integration/actions/workflows/build_and_test.yml/badge.svg?branch=main
[build-url]: https://github.com/Genocs/telegram-integration/actions/workflows/build_and_test.yml
[package-shield]: https://img.shields.io/badge/nuget-v.1.0.0-blue?&label=latests&logo=nuget
[package-url]: https://github.com/Genocs/telegram-integration/actions/workflows/build_and_test.yml
[downloads-shield]: https://img.shields.io/nuget/dt/Genocs.Telegram.Integration.svg?color=2da44e&label=downloads&logo=nuget
[downloads-url]: https://www.nuget.org/packages/Genocs.Telegram.Integration
[contributors-shield]: https://img.shields.io/github/contributors/Genocs/telegram-integration.svg?style=flat-square
[contributors-url]: https://github.com/Genocs/telegram-integration/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Genocs/telegram-integration?style=flat-square
[forks-url]: https://github.com/Genocs/telegram-integration/network/members
[stars-shield]: https://img.shields.io/github/stars/Genocs/telegram-integration.svg?style=flat-square
[stars-url]: https://img.shields.io/github/stars/Genocs/telegram-integration?style=flat-square
[issues-shield]: https://img.shields.io/github/issues/Genocs/telegram-integration?style=flat-square
[issues-url]: https://github.com/Genocs/telegram-integration/issues
[discord-shield]: https://img.shields.io/discord/1106846706512953385?color=%237289da&label=Discord&logo=discord&logoColor=%237289da&style=flat-square
[discord-url]: https://discord.com/invite/fWwArnkV
[gitter-shield]: https://img.shields.io/badge/chat-on%20gitter-blue.svg
[gitter-url]: https://gitter.im/genocs/
[twitter-shield]: https://img.shields.io/twitter/follow/genocs?color=1DA1F2&label=Twitter&logo=Twitter&style=flat-square
[twitter-url]: https://twitter.com/genocs
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=flat-square&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/giovanni-emanuele-nocco-b31a5169/
[twitterx-shield]: https://img.shields.io/twitter/url/https/twitter.com/genocs.svg?style=social
[twitterx-url]: https://twitter.com/genocs




<p align="center">
    <img src="./assets/genocs-library-logo.png" alt="icon">
</p>

# Telegram Integration Library 
.NET7.0 Library to be used to implement Telegram integration.

## Goals

The goal of this repository is to help developers/companies kickstart their Web Application Development with a pre-built Blazor WebAssembly Template that includes several much needed components and features.

> Note that this is a frontend/client application only! The backend for this application is available in a seperate repository. 
> - Find Genocs's .NET Web API template here - [microservice-template](https://github.com/Genocs/microservice-template)

## Prerequisites

- Make sure you have the [Genocs Backend]( https://github.com/Genocs/microservice-template) API Running.
- Once Genocs's .NET Web API is up and running, run the Blazor WebAssembly Project to consume it's services.

## Getting Started

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
docker build -f webapi.dockerfile -t genocs/telegram_integration-webapi:2.1.0 -t genocs/telegram_integration-webapi:latest .
docker build -f worker.dockerfile -t genocs/telegram_integration-worker:2.1.0 -t genocs/telegram_integration-worker:latest .

# Push image on dockerhub
docker push genocs/telegram_integration-webapi:2.1.0
docker push genocs/telegram_integration-webapi:latest

docker push genocs/telegram_integration-worker:2.1.0
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

api-workbench.rest
Use this file inside Visual Studio code with [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) plugin
For more details on getting started, [read the documentation](https://genocs-blog.netlify.app/blazor-template/general/overview/)

## License

This project is licensed with the [MIT license](LICENSE).

## Changelogs

View Complete [Changelogs](https://github.com/Genocs/microservice-template/blob/main/CHANGELOGS.md).

## Community

- Discord [@genocs](https://discord.com/invite/fWwArnkV)
- Facebook Page [@genocs](https://facebook.com/Genocs)
- Youtube Channel [@genocs](https://youtube.com/c/genocs)


## Support

Has this Project helped you learn something New? or Helped you at work?
Here are a few ways by which you can support.

- ⭐ Leave a star! 
- 🥇 Recommend this project to your colleagues.
- 🦸 Do consider endorsing me on LinkedIn for ASP.NET Core - [Connect via LinkedIn](https://www.linkedin.com/in/giovanni-emanuele-nocco-b31a5169/) 
- ☕ If you want to support this project in the long run, [consider buying me a coffee](https://www.buymeacoffee.com/genocs)!
  

[![buy-me-a-coffee](https://raw.githubusercontent.com/Genocs/blazor-template/main/assets/buy-me-a-coffee.png "buy-me-a-coffee")](https://www.buymeacoffee.com/genocs)

## Code Contributors

This project exists thanks to all the people who contribute. [Submit your PR and join the team!](CONTRIBUTING.md)

[![genocs contributors](https://contrib.rocks/image?repo=Genocs/blazor-template "genocs contributors")](https://github.com/genocs/blazor-template/graphs/contributors)

## Financial Contributors

Become a financial contributor and help me sustain the project. [Support the Project!](https://opencollective.com/genocs/contribute)

<a href="https://opencollective.com/genocs"><img src="https://opencollective.com/genocs/individuals.svg?width=890"></a>


## Acknowledgements
