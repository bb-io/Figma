using Apps.Figma.Handlers;
using Apps.Figma.Models.Requests;
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
        var handler = new FileNodeHandler(InvocationContext, new FileKeyRequest { ContentId = key });

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task Variable_Collection_Handler()
    {
        var key = "KazRrZ8qMRapzOJZ08J7FP";
        var handler = new VariableCollectionHandler(InvocationContext, new FileKeyRequest { ContentId = key });

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task Variable_Mode_Handler()
    {
        var key = "KazRrZ8qMRapzOJZ08J7FP";
        var collectionId = "VariableCollectionId:1122:208";
        var handler = new VariableModeHandler(InvocationContext, new FileKeyRequest { ContentId = key }, new VariableDownloadRequest { CollectionId = collectionId });

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }

        Assert.IsTrue(result.Count() > 0);
    }
}
