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
.NET10.0 Library to be used to implement Telegram integration.

## Goals

The library is based on the [Telegram Bot API](https://core.telegram.org/bots/api).
The goal of this repository is to help developers/companies to implement solution based on Telegram Integration with OpenAI services.
The solution contains integration with the following services:
 - **OpenAI** for the implementation of a chatbot that can be used to interact with the user.
 - **Stripe** for the implementation of a payment system that can be used to interact with the user.
 - **MongoDB** for the implementation of a storage system that can be used to store the user data.
 - **CloudAmqp** for the implementation of a message broker that can be used to send messages between the services.

 > **Note:**
 > The library do not contains reference to ETL (Extraction Transformation and Loading).
 >
 > ETL is out of the scope of this library and is moved on other repository.


## Prerequisites
Make sure you have following accounts:
- [Telegram](https://telegram.org/) account
- [Azure](https://azure.microsoft.com/) account
- [OpenAI](https://openai.com/) account
- [NGROK](https://ngrok.com/) account
- [Stripe](https://stripe.com/) account
- [MongoDB](https://www.mongodb.com/) account
- [CloudAmqp](https://www.cloudamqp.com/) account

On Azure you need to create the following resources:
- Azure Cognitive Services account
- Azure Storage account

## Getting Started

``` PS
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Run webapi
dotnet run --project .\src\Genocs.TelegramIntegration.WebApi

# Run worker
dotnet run --project .\src\Genocs.TelegramIntegration.Worker

# ** DOCKER ** #
# Build docker image
docker build -f webapi.dockerfile -t genocs/telegram_integration-webapi:3.0.1 -t genocs/telegram_integration-webapi:latest .
docker build -f worker.dockerfile -t genocs/telegram_integration-worker:3.0.1 -t genocs/telegram_integration-worker:latest .

# Push image on dockerhub
docker push genocs/telegram_integration-webapi:3.0.1
docker push genocs/telegram_integration-webapi:latest

docker push genocs/telegram_integration-worker:3.0.1
docker push genocs/telegram_integration-worker:latest

# Run with docker compose
docker-compose up
```


## How to use NGROK to expose the service to the internet

- Download ngrok from https://ngrok.com/download
- unzip the file
- run the command: ngrok http https://localhost:5091
- copy the https url and use it to configure the webhook

## License

This project is licensed with the [MIT license](LICENSE).

## Changelogs

View Complete [Changelogs](https://github.com/Genocs/microservice-template/blob/main/CHANGELOG.md).

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
