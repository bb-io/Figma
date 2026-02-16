using Apps.Figma.Handlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Tests.Figma.Base;

namespace Tests.Figma;

[TestClass]
public class HandlerTests : TestBase
{
    [TestMethod]
    public async Task File_key_handler()
    {
        var handler = new FileKeyHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task File_node_handler()
    {
        var key = "cF2FKeD8oh09AQF3Z0iwPF";
        var handler = new FileNodeHandler(InvocationContext, new Apps.Figma.Models.Requests.FileKeyRequest { Key = key });

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }

        Assert.IsTrue(result.Count() > 0);
    }
}
