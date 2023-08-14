using Genocs.TelegramIntegration.Services;
using Genocs.TelegramIntegration.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Genocs.TelegramIntegration.xUnitTests;

public class ClientUnitTests
{
    [Fact]
    public void ConnectToClientTest()
    {
        Options.TelegramSettings telegramOptions = new Options.TelegramSettings() { Token = "TelegramSettingOption" };
        var mockTelegramOptions = new Mock<IOptions<Options.TelegramSettings>>();

        // We need to set the Value of IOptions to be the Options.TelegramSettings Class
        mockTelegramOptions.Setup(ap => ap.Value).Returns(telegramOptions);

        Options.ApiClientSettings apiClientOptions = new Options.ApiClientSettings() { FormRecognizerUrl = "TelegramSettingOption" };
        var mockApiClientOptions = new Mock<IOptions<Options.ApiClientSettings>>();

        // We need to set the Value of IOptions to be the Options.ApiClientSettings Class
        mockApiClientOptions.Setup(ap => ap.Value).Returns(apiClientOptions);


        // mock logger
        var mockLogger = new Mock<ILogger<TelegramProxy>>();


        ITelegramProxy proxy = new TelegramProxy(mockTelegramOptions.Object, mockLogger.Object, null, mockApiClientOptions.Object, null, null);
        proxy.PullUpdatesAsync();

    }
}