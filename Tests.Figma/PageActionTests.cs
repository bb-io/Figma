using Apps.Figma.Actions;
using Apps.Figma.Models.Requests;
using Newtonsoft.Json;
using Tests.Figma.Base;

namespace Tests.Figma;

[TestClass]
public class PageActionIntegrationTests : TestBase
{
    [TestMethod]
    public async Task Search_pages_in_file()
    {
        // Arrange
        var actions = new PageActions(InvocationContext);
        var key = new FileKeyRequest { ContentId = "cF2FKeD8oh09AQF3Z0iwPF" };
        var filter = new SearchPagesRequest { PageNameContains = "dEsIgNs" };

        // Act
        var response = await actions.SearchPages(key, filter);

        // Assert
        Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        Assert.IsTrue(response.Pages.Any(page => page.PageId == "1:42"));
    }
}
