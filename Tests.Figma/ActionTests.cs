using Apps.Figma.Actions;
using Apps.Figma.Models.Requests;
using Blackbird.Filters.Constants;
using Blackbird.Filters.Enums;
using Blackbird.Filters.Transformations;
using Newtonsoft.Json;
using System.Net.Mime;
using Tests.Figma.Base;

namespace Tests.Figma;

[TestClass]
public class ActionTests : TestBase
{
    [TestMethod]
    public async Task Download_image()
    {
        var actions = new ImageActions(InvocationContext, new FileManager());
        var key = "cF2FKeD8oh09AQF3Z0iwPF";
        var nodeId = "1745-6243";

        var result = await actions.DownloadImage(
            new FileKeyRequest { ContentId = key }, 
            new FileNodeRequest { NodeId = nodeId }, 
            new ImageDownloadOptions { });

        Console.WriteLine(result.Url);
        Assert.IsNotNull(result.File);

        await FileManager.DownloadFileAsync(result.File, result.Url);

    }

    [TestMethod]
    public async Task Download_local_variables()
    {
        var actions = new VariableActions(InvocationContext, new FileManager());
        var key = "KazRrZ8qMRapzOJZ08J7FP";

        var response = await actions.DownloadVariables(new FileKeyRequest { ContentId = key }, new VariableDownloadRequest { CollectionId = "VariableCollectionId:1122:208", ModeName = "en-us" });
        Assert.IsNotNull(response.Content);

        using var contentStream = FileManager.ReadOutputAsStream(response.Content);

        var transformation = Transformation.Load(contentStream, response.Content.Name, response.Content.ContentType)?.Value ?? throw new Exception("Failed to load");
        Console.WriteLine(JsonConvert.SerializeObject(transformation.SourceSystemReference, Formatting.Indented));
        
        foreach(var unit in transformation.GetUnits())
        {
            Console.WriteLine($"{unit.Key}: {unit.GetSource().GetCodedText()}");
        }

        Assert.AreEqual(transformation.SourceSystemReference.SystemName, "Figma");
    }

    [TestMethod]
    public async Task Translate_variables()
    {
        var actions = new VariableActions(InvocationContext, new FileManager());
        var key = "KazRrZ8qMRapzOJZ08J7FP";
        var collectionId = "VariableCollectionId:1122:208";
        var targetMode = "nl-be";     

        var response = await actions.UploadVariables(new VariableUploadRequest
        {
            Content = new()
            {
                Name = "Variables - Locales.json",
                ContentType = MediaTypeNames.Application.Json,
            },
            CollectionId = collectionId,
            //Locale = targetMode,
            ContentId = key
        });

        Console.WriteLine($"Number of updated variables: {response.NumberOfUpdatedVariables}");
        Console.WriteLine($"Number of created variables: {response.NumberOfNewVariables}");

        using var contentStream = FileManager.ReadOutputAsStream(response.Content);

        var transformation = Transformation.Load(contentStream, response.Content.Name, response.Content.ContentType)?.Value ?? throw new Exception("Failed to load");
        Console.WriteLine(JsonConvert.SerializeObject(transformation.SourceSystemReference, Formatting.Indented));

        foreach (var unit in transformation.GetUnits())
        {
            Console.WriteLine($"{unit.GetSource().GetCodedText()} - {unit.GetTarget().GetCodedText()}");
        }

        Assert.AreEqual("Figma", transformation.SourceSystemReference.SystemName);
    }
}
