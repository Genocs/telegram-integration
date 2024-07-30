using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Configurations;
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
    // [Fact]
    public void ConnectToClientTest()
    {
        var mockTelegramOptions = new Mock<IOptions<TelegramSettings>>();

        // We need to set the Value of IOptions to be the Options.TelegramSettings Class
        TelegramSettings telegramOptions = new TelegramSettings() { Token = "TelegramSettingOption" };
        mockTelegramOptions.Setup(ap => ap.Value).Returns(telegramOptions);

        var mockOpenAIMiddleware = new Mock<IOpenAIMiddleware>();
        var mockApiClientOptions = new Mock<IOptions<ApiClientSettings>>();

        // We need to set the Value of IOptions to be the Options.ApiClientSettings Class
        ApiClientSettings apiClientOptions = new ApiClientSettings() { FormRecognizerUrl = "TelegramSettingOption" };
        mockApiClientOptions.Setup(ap => ap.Value).Returns(apiClientOptions);

        var mockStripeOptions = new Mock<IOptions<StripeSettings>>();

        // We need to set the Value of IOptions to be the Options.ApiClientSettings Class
        StripeSettings stripeOptions = new StripeSettings() { Token = "StripeOption" };
        mockStripeOptions.Setup(ap => ap.Value).Returns(stripeOptions);

        // mock logger
        var mockLogger = new Mock<ILogger<TelegramProxy>>();

        var mockDistributedCache = new Mock<IDistributedCache>();

        var mockHttpClientFactory = new Mock<IHttpClientFactory>();

        var mockChatUpdateRepository = new Mock<IMongoDbRepository<ChatUpdate>>();

        ITelegramProxy proxy = new TelegramProxy(
                                                 mockTelegramOptions.Object,
                                                 mockLogger.Object,
                                                 mockOpenAIMiddleware.Object,
                                                 mockApiClientOptions.Object,
                                                 mockStripeOptions.Object,
                                                 mockHttpClientFactory.Object,
                                                 mockChatUpdateRepository.Object);
        proxy.PullUpdatesAsync();

    }
}