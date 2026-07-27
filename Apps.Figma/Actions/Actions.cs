using Apps.Figma.Models.Dto;
using Apps.Figma.Models.Requests;
using Apps.Figma.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Coders;
using Blackbird.Filters.Constants;
using Blackbird.Filters.Enums;
using Blackbird.Filters.Shared;
using Blackbird.Filters.Transformations;
using RestSharp;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Reflection;

namespace Apps.Figma.Actions;

[ActionList]
public class Actions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    [Action("Download image", Description = "Download an image from a Figma file")]
    public async Task<ImageResponse> DownloadImage(
        [ActionParameter] FileKeyRequest keyRequest, 
        [ActionParameter] FileNodeRequest nodeRequest, 
        [ActionParameter] ImageDownloadOptions options
        )
    {
        if (string.IsNullOrEmpty(keyRequest.ContentId)) throw new PluginMisconfigurationException("The key input is null or empty.");
        if (string.IsNullOrEmpty(nodeRequest.NodeId)) throw new PluginMisconfigurationException("The node ID input is null or empty.");

        var mediaTypes = new Dictionary<string, string>()
        {
            { "png", MediaTypeNames.Image.Png },
            { "jpg", MediaTypeNames.Image.Jpeg },
            { "svg", MediaTypeNames.Image.Svg },
            { "pdf", MediaTypeNames.Application.Pdf }
        };

        if (options.Format is not null && !mediaTypes.ContainsKey(options.Format))
        {
            throw new PluginMisconfigurationException($"{options.Format} is an unsupported format.");
        }

        var request = new RestRequest($"/v1/images/{keyRequest.ContentId}", Method.Get);
        request.AddQueryParameter("ids", nodeRequest.NodeId);
        if (options.Format is not null)
        {
            request.AddQueryParameter("format", options.Format);
        }
        var response = await Client.ExecuteWithErrorHandling<ImageMapDto>(request);

        if (response.ImageMap.Count == 0)
        {
            throw new PluginMisconfigurationException("No images found in the response, please check if your key and node ID are correct.");
        }

        var image = response.ImageMap.FirstOrDefault();

        var imageName = image.Value.Split('/').Last();
        var extension = options.Format ?? "png";

        return new ImageResponse()
        {
            NodeId = image.Key,
            Url = image.Value,
            File = new FileReference(new HttpRequestMessage(HttpMethod.Get, image.Value), $"{imageName}.{extension}", mediaTypes[extension]),
        };
    }

    private async Task<FigmaFileDto> GetFileInfo(string key)
    {
        var request = new RestRequest($"/v1/files/{key}/meta", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<FigmaFileMetaDto>(request);
        if (response.File is null) throw new PluginMisconfigurationException($"Cannot find Figma file with key {key}");
        return response.File;
    }

    private async Task<Models.Dto.Meta> GetFileVariables(string key)
    {
        var request = new RestRequest($"/v1/files/{key}/variables/local", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<VariablesResponseDto>(request);
        return response.Meta;
    }

    [BlueprintActionDefinition(BlueprintAction.DownloadContent)]
    [Action("Download variables", Description = "Download the variables of a Figma file")]
    public async Task<FileResponse> DownloadVariables([ActionParameter] FileKeyRequest keyRequest, [ActionParameter] VariableDownloadRequest variableRequest)
    {      
        if (string.IsNullOrEmpty(keyRequest.ContentId)) throw new PluginMisconfigurationException("The key input is null or empty.");
        if (string.IsNullOrEmpty(variableRequest.CollectionId)) throw new PluginMisconfigurationException("The collection ID input is null or empty.");
        if (string.IsNullOrEmpty(variableRequest.TargetMode)) throw new PluginMisconfigurationException("The target language input is null or empty.");

        var fileInfo = await GetFileInfo(keyRequest.ContentId);
        var variablesMeta = await GetFileVariables(keyRequest.ContentId);

        if (!variablesMeta.VariableCollections.TryGetValue(variableRequest.CollectionId, out var collection)) throw new PluginMisconfigurationException($"Cannot find collection with ID '{variableRequest.CollectionId}'");
        var targetMode = collection.Modes.FirstOrDefault(x => x.ModeId == variableRequest.TargetMode) ?? throw new PluginMisconfigurationException($"Cannot find mode with ID '{variableRequest.TargetMode}'");

        var sourceModeId = variableRequest.SourceMode ?? collection.DefaultModeId;
        var sourceMode = collection.Modes.FirstOrDefault(x => x.ModeId == sourceModeId) ?? throw new PluginMisconfigurationException($"Cannot find mode with ID '{sourceModeId}'");

        var transformation = new Transformation(sourceMode.Name, targetMode.Name);
        transformation.OriginalName = collection.Name;
        transformation.SourceSystemReference = new SystemReference
        {
            ContentId = $"{keyRequest.ContentId}|{collection.Id}",
            ContentName = $"{fileInfo.Name} - {collection.Name}",
            AdminUrl = $"https://www.figma.com/design/{keyRequest.ContentId}?view=variables",
            SystemName = "Figma",
            SystemRef = "https://www.figma.com"
        };

        var coder = new PlaintextCoder();
        foreach (var variable in variablesMeta.Variables.Values.Where(x => x.VariableCollectionId == collection.Id))
        {
            variable.ValuesByMode.TryGetValue(sourceMode.ModeId, out var sourceVariable);
            variable.ValuesByMode.TryGetValue(targetMode.ModeId, out var targetVariable);

            if (sourceVariable is not string && sourceVariable is not null) throw new PluginMisconfigurationException($"{sourceMode.Name} is not a string type");
            if (targetVariable is not string && targetVariable is not null) throw new PluginMisconfigurationException($"{targetMode.Name} is not a string type");

            var segment = new Segment(coder);
            var sourceString = (string?)sourceVariable ?? string.Empty;
            var targetString = (string?)targetVariable ?? string.Empty;

            segment.Source = coder.DeserializeSegment(sourceString);
            segment.Target = coder.DeserializeSegment(targetString);

            segment.State = SegmentState.Initial;

            var unit = new Unit(coder);
            unit.Segments.Add(segment);
            unit.Name = variable.Id;
            unit.MetaData.Add(new Metadata("Figma name", variable.Name));
            unit.MetaData.Add(new Metadata("key", variable.Name) { Category = ["blackbird"] });
            unit.Notes.Add(new Note(variable.Description));

            transformation.Children.Add(unit);
        }

        return new FileResponse
        {
            Content = await fileManagementClient.UploadAsync(transformation.ToStream(), MediaTypes.Xliff2, transformation.BilingualFileName),
        };
    }

    [BlueprintActionDefinition(BlueprintAction.UploadContent)]
    [Action("Upload variables", Description = "Upload translated variables back to a Figma file")]
    public async Task<UploadVariablesResponse> UploadVariables([ActionParameter] VariableUploadRequest variableRequest)
    {
        if (string.IsNullOrEmpty(variableRequest.ContentId)) throw new PluginMisconfigurationException("The key input is null or empty.");
        if (string.IsNullOrEmpty(variableRequest.CollectionId)) throw new PluginMisconfigurationException("The collection ID input is null or empty.");

        var fileInfo = await GetFileInfo(variableRequest.ContentId);
        var variablesMeta = await GetFileVariables(variableRequest.ContentId);

        if (!variablesMeta.VariableCollections.TryGetValue(variableRequest.CollectionId, out var collection)) throw new PluginMisconfigurationException($"Cannot find collection with ID '{variableRequest.CollectionId}'");
        var targetMode = collection.Modes.FirstOrDefault(x => x.ModeId == variableRequest.Locale) ?? throw new PluginMisconfigurationException($"Cannot find mode with ID '{variableRequest.Locale}'");

        var currentVariables = variablesMeta.Variables.Values.Where(x => x.VariableCollectionId == collection.Id);

        var file = await fileManagementClient.DownloadAsync(variableRequest.Content);
        var transformationResult = Transformation.Load(file, variableRequest.Content.Name, variableRequest.Content.ContentType);
        if (!transformationResult.Success)
        {
            throw new PluginMisconfigurationException(transformationResult.Error);
        }
        var transformation = transformationResult.Value!;

        var variableModeValues = new List<VariableModeValueDto>();
        var updatedVariablesCount = 0;

        foreach(var variable in currentVariables)
        {
            variable.ValuesByMode.TryGetValue(targetMode.ModeId, out var targetVariable);
            if (targetVariable is not string && targetVariable is not null) throw new PluginMisconfigurationException($"{targetMode.Name} is not a string type");
            var targetString = (string?)targetVariable ?? string.Empty;

            var targetUnit = transformation.GetUnits().FirstOrDefault(x => x.Name == variable.Id);
            if (targetUnit is null) continue;

            var targetUnitText = targetUnit.GetTarget().GetCodedText();
            if (targetUnitText == targetString) continue;

            variableModeValues.Add(new VariableModeValueDto
            {
                VariableId = targetUnit.Name,
                ModeId = targetMode.ModeId,
                Value = targetUnitText,
            });

            updatedVariablesCount++;
        }

        if (variableModeValues.Count > 0)
        {
            var request = new RestRequest($"/v1/files/{variableRequest.ContentId}/variables", Method.Post);
            request.AddJsonBody(new
            {
                VariableModeValues = variableModeValues
            });
            await Client.ExecuteWithErrorHandling(request);
        }        

        transformation.TargetSystemReference = new SystemReference
        {
            ContentId = $"{variableRequest.ContentId}|{collection.Id}",
            ContentName = $"{fileInfo.Name} - {collection.Name}",
            AdminUrl = $"https://www.figma.com/design/{variableRequest.ContentId}?view=variables",
            SystemName = "Figma",
            SystemRef = "https://www.figma.com"
        };
        transformation.TargetLanguage = targetMode.Name;

        return new UploadVariablesResponse
        {
            Content = await fileManagementClient.UploadAsync(transformation.ToStream(), MediaTypes.Xliff2, transformation.BilingualFileName),
            NumberOfUpdatedVariables = updatedVariablesCount,
        };
    }
}
