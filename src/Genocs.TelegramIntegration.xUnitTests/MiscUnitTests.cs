namespace Genocs.TelegramIntegration.xUnitTests;

public class MiscUnitTests
{
    [Fact]
    public void InterpolateStringUnitTest()
    {
        string template = "https://barcode.tec-it.com/barcode.ashx?data={command.ImageUrl}&code=Code128";
        string result = template.Replace("{command.ImageUrl}", "1234567890");
        Assert.Equal("https://barcode.tec-it.com/barcode.ashx?data=1234567890&code=Code128", result);
    }

    [Fact]
    public void InterpolateStringInPlaceUnitTest()
    {
        string template = "https://barcode.tec-it.com/barcode.ashx?data={0}&code=Code128";
        string result = string.Format(template, "1234567890");
        Assert.Equal("https://barcode.tec-it.com/barcode.ashx?data=1234567890&code=Code128", result);
    }
}
