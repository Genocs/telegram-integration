using Microsoft.Extensions.Options;
using Moq;

namespace Genocs.TelegramIntegration.xUnitTests;

public class OptionsUnitTests
{
    [Fact]
    public void ConnectToClientTest()
    {
        var mockTelegramOptions = new Mock<IOptions<Options.TelegramSettings>>();

        // We need to set the Value of IOptions to be the Options.TelegramSettings Class
        Options.TelegramSettings telegramOptions = new Options.TelegramSettings();
        mockTelegramOptions.Setup(ap => ap.Value).Returns(telegramOptions);

        var mockOpenAISettings = new Mock<IOptions<Options.OpenAISettings>>();
    }
}