using Genocs.TelegramIntegration.Services;
using Genocs.TelegramIntegration.Services.Interfaces;

namespace Genocs.TelegramIntegration.xUnitTests;

public class ClientUnitTests
{
    [Fact]
    public void ConnectToClientTest()
    {
        ITelegramProxy proxy = new TelegramProxy(null, null, null, null, null);
        proxy.PullUpdatesAsync();

    }
}