using Apps.Figma.Actions;
using Apps.Figma.Handlers;
using Apps.Figma.Models.Requests;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Newtonsoft.Json;
using Tests.Figma.Base;

namespace Tests.Figma;

[TestClass]
public class FrameImageTests : TestBase
{
    [TestMethod]
    public async Task Page_selector_and_frame_export()
    {
        // Arrange
        var key = new FileKeyRequest { ContentId = "cF2FKeD8oh09AQF3Z0iwPF" };
        var handler = new PageHandler(InvocationContext, key);
        var page = (await handler.GetDataAsync(new DataSourceContext(), CancellationToken.None)).First();
        var actions = new ImageActions(InvocationContext, FileManager);

        // Act
        var result = await actions.DownloadImagesFromFrames(key, new PageRequest { PageId = page.Value }, new ImageDownloadOptions());
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

        // Assert
        Assert.IsTrue(result.Images.Any(image => !string.IsNullOrWhiteSpace(image.NodeId)));
    }
}
