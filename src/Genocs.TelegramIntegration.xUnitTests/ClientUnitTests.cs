using Genocs.Integration.CognitiveServices.Interfaces;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services;
using Genocs.TelegramIntegration.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Genocs.TelegramIntegration.xUnitTests;

public class ClientUnitTests
{
    [Fact]
    public void ConnectToClientTest()
    {
        var mockTelegramOptions = new Mock<IOptions<Options.TelegramSettings>>();

        // We need to set the Value of IOptions to be the Options.TelegramSettings Class
        Options.TelegramSettings telegramOptions = new Options.TelegramSettings() { Token = "TelegramSettingOption" };
        mockTelegramOptions.Setup(ap => ap.Value).Returns(telegramOptions);

        var mockOpenAISettings = new Mock<IOptions<Options.OpenAISettings>>();

        // We need to set the Value of IOptions to be the Options.OpenAISettings Class
        Options.OpenAISettings openAIOptions = new Options.OpenAISettings() { APIKey = "APIKey", Url = "Url" };
        mockOpenAISettings.Setup(ap => ap.Value).Returns(openAIOptions);

        var mockApiClientOptions = new Mock<IOptions<Options.ApiClientSettings>>();

        // We need to set the Value of IOptions to be the Options.ApiClientSettings Class
        Options.ApiClientSettings apiClientOptions = new Options.ApiClientSettings() { FormRecognizerUrl = "TelegramSettingOption" };
        mockApiClientOptions.Setup(ap => ap.Value).Returns(apiClientOptions);

        // mock logger
        var mockLogger = new Mock<ILogger<TelegramProxy>>();

        var mockDistributedCache = new Mock<IDistributedCache>();

        var mockFormRecognizer = new Mock<IFormRecognizer>();
        var mockImageClassifier = new Mock<IImageClassifier>();
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();

        var mockMongoDbRepositoryGenocsChat = new Mock<IMongoDbRepository<GenocsChat>>();

        ITelegramProxy proxy = new TelegramProxy(
                                                 mockTelegramOptions.Object,
                                                 mockLogger.Object,
                                                 mockOpenAISettings.Object,
                                                 mockApiClientOptions.Object,
                                                 mockHttpClientFactory.Object,
                                                 mockMongoDbRepositoryGenocsChat.Object,
                                                 mockFormRecognizer.Object,
                                                 mockImageClassifier.Object,
                                                 mockDistributedCache.Object);
        proxy.PullUpdatesAsync();

    }
}