using Apps.Figma.Actions;
using Apps.Figma.Models.Requests;
using Tests.Figma.Base;

namespace Tests.Figma;

[TestClass]
public class ActionTests : TestBase
{
    [TestMethod]
    public async Task Download_image()
    {
        var actions = new Actions(InvocationContext);
        var key = "cF2FKeD8oh09AQF3Z0iwPF";
        var nodeId = "1745-6243";

        var result = await actions.DownloadImage(
            new FileKeyRequest { Key = key }, 
            new FileNodeRequest { NodeId = nodeId }, 
            new ImageDownloadOptions { });

        Console.WriteLine(result.Url);
        Assert.IsNotNull(result.File);

        await FileManager.DownloadFileAsync(result.File, result.Url);

    }
}
