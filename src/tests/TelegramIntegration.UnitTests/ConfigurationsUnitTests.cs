using Genocs.TelegramIntegration.Configurations;
using Microsoft.Extensions.Options;
using Moq;

namespace Genocs.TelegramIntegration.xUnitTests;

public class ConfigurationsUnitTests
{
    [Fact]
    public void ConnectToClientTest()
    {
        var mockTelegramOptions = new Mock<IOptions<TelegramSettings>>();

        // We need to set the Value of IOptions to be the Options.TelegramSettings Class
        TelegramSettings telegramOptions = new TelegramSettings();
        mockTelegramOptions.Setup(ap => ap.Value).Returns(telegramOptions);

        var mockOpenAISettings = new Mock<IOptions<OpenAISettings>>();
    }
}