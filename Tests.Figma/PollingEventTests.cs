using Apps.Figma.Events;
using Apps.Figma.Models.Polling;
using Blackbird.Applications.Sdk.Common.Polling;
using Newtonsoft.Json;
using Tests.Figma.Base;

namespace Tests.Figma;

[TestClass]
public class PollingEventTests : TestBase
{
    private VariablePollingList PollingEvents => new VariablePollingList(InvocationContext);

    [TestMethod]
    public async Task OnVariablesUpdated_NullMemory_DoesNotFIre()
    {
        var key = "KazRrZ8qMRapzOJZ08J7FP";
        var collectionId = "VariableCollectionId:1122:208";

        var request = new PollingEventRequest<VariablesPollingMemory>
        {
            Memory = new VariablesPollingMemory { VariablesHashByMode = null }
        };
        var result = await PollingEvents.OnVariablesUpdated(request, new VariablePollingFilters() { CollectionId = collectionId, ContentId = key});

        Assert.IsFalse(result.FlyBird, "The first poll must only establish a baseline.");
        Assert.IsNull(result.Result);
        Assert.IsNotNull(result.Memory?.VariablesHashByMode, "Memory must be advanced on the first poll.");

        Console.WriteLine(JsonConvert.SerializeObject(result.Memory.VariablesHashByMode, Formatting.Indented));
    }

    [TestMethod]
    public async Task OnVariablesUpdated_WithMemory_fires()
    {
        var key = "KazRrZ8qMRapzOJZ08J7FP";
        var collectionId = "VariableCollectionId:1122:208";

        var memory = new VariablesPollingMemory
        {
            VariablesHashByMode = new Dictionary<string, string>
            {
                  { "en-us", "A92F43257B8168979D94F6632494D547694CA3831A906B2747BC7B5223BE2663" },
                  { "uk-ua", "211155AD7D09B81C435C8815FB133EC7E408A8CEE3845AF574DEB2D91424A666" },
                  { "nl-nl", "90B7AC973EDE3EB7CC001D05E43A15D6854C24BDC3889387C80BE533CCCA95B1" },
                  { "de-de", "4E24FC788F509AC2BBE38B65E02CF90DF330A13AE7F6478D7962216672D2EBBA" },
                  { "nl-be", "E7B1D5AF164410560995E24743A74C35DBC691BB263F238CFC38D2BF569EF53A" }
            }
        };

        var request = new PollingEventRequest<VariablesPollingMemory>
        {
            Memory = memory,
        };
        var result = await PollingEvents.OnVariablesUpdated(request, new VariablePollingFilters() { CollectionId = collectionId, ContentId = key });

        Assert.IsTrue(result.FlyBird, "The Bird did not fly");
        Assert.IsNotNull(result.Result);
        Assert.IsNotNull(result.Memory?.VariablesHashByMode, "Memory must exist.");
        Assert.AreNotEqual(memory.VariablesHashByMode, result.Memory?.VariablesHashByMode);

        Console.WriteLine(JsonConvert.SerializeObject(result.Memory!.VariablesHashByMode, Formatting.Indented));
    }

    [TestMethod]
    public async Task OnVariablesUpdated_WithMemory_limited_modes_doesnotFire()
    {
        var key = "KazRrZ8qMRapzOJZ08J7FP";
        var collectionId = "VariableCollectionId:1122:208";

        var memory = new VariablesPollingMemory
        {
            VariablesHashByMode = new Dictionary<string, string>
            {
                  { "en-us", "A92F43257B8168979D94F6632494D547694CA3831A906B2747BC7B5223BE2663" },
                  { "uk-ua", "211155AD7D09B81C435C8815FB133EC7E408A8CEE3845AF574DEB2D91424A666" },
                  { "nl-nl", "90B7AC973EDE3EB7CC001D05E43A15D6854C24BDC3889387C80BE533CCCA95B1" },
                  { "de-de", "4E24FC788F509AC2BBE38B65E02CF90DF330A13AE7F6478D7962216672D2EBBA" },
                  { "nl-be", "E7B1D5AF164410560995E24743A74C35DBC691BB263F238CFC38D2BF569EF53A" }
            }
        };

        var request = new PollingEventRequest<VariablesPollingMemory>
        {
            Memory = memory,
        };
        var result = await PollingEvents.OnVariablesUpdated(request, new VariablePollingFilters() { CollectionId = collectionId, ContentId = key, ModeNames = ["not-there"] });

        Assert.IsFalse(result.FlyBird, "The first poll must only establish a baseline.");
        Assert.IsNull(result.Result);
    }

    [TestMethod]
    public async Task OnVariablesUpdated_WithMemory_with_limited_modes_returns_subset()
    {
        var key = "KazRrZ8qMRapzOJZ08J7FP";
        var collectionId = "VariableCollectionId:1122:208";

        var memory = new VariablesPollingMemory
        {
            VariablesHashByMode = new Dictionary<string, string>
            {
                  { "en-us", "A92F43257B8168979D94F6632494D547694CA3831A906B2747BC7B5223BE2663" },
                  { "uk-ua", "211155AD7D09B81C435C8815FB133EC7E408A8CEE3845AF574DEB2D91424A666" },
                  { "nl-nl", "90B7AC973EDE3EB7CC001D05E43A15D6854C24BDC3889387C80BE533CCCA95B1" },
                  { "de-de", "4E24FC788F509AC2BBE38B65E02CF90DF330A13AE7F6478D7962216672D2EBBA" },
                  { "nl-be", "E7B1D5AF164410560995E24743A74C35DBC691BB263F238CFC38D2BF569EF53A" }
            }
        };

        var request = new PollingEventRequest<VariablesPollingMemory>
        {
            Memory = memory,
        };
        var result = await PollingEvents.OnVariablesUpdated(request, new VariablePollingFilters() { CollectionId = collectionId, ContentId = key, ModeNames = ["en-us", "uk-ua"] });

        Assert.IsTrue(result.FlyBird, "The Bird did not fly");
        Assert.IsNotNull(result.Result);
        Assert.IsNotNull(result.Memory?.VariablesHashByMode, "Memory must exist.");
        Assert.AreNotEqual(memory.VariablesHashByMode, result.Memory?.VariablesHashByMode);
        
        Console.WriteLine(JsonConvert.SerializeObject(result.Result, Formatting.Indented));

        Assert.IsTrue(result.Result.Items.Count == 2);
    }
}
