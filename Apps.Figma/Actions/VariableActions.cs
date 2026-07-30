using Apps.Figma.Models.Dto;
using Apps.Figma.Models.Requests;
using Apps.Figma.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Coders;
using Blackbird.Filters.Constants;
using Blackbird.Filters.Shared;
using Blackbird.Filters.Transformations;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Mime;

namespace Apps.Figma.Actions;
[ActionList("Variables")]
public class VariableActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    private async Task<FigmaFileDto> GetFileInfo(string key)
    {
        var request = new RestRequest($"/v1/files/{key}/meta", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<FigmaFileMetaDto>(request);
        if (response.File is null) throw new PluginMisconfigurationException($"Cannot find Figma file with key {key}");
        return response.File;
    }    

    [BlueprintActionDefinition(BlueprintAction.DownloadContent)]
    [Action("Download variables", Description = "Download variables from a Figma project")]
    public async Task<FileResponse> DownloadVariables([ActionParameter] VariableDownloadRequest variableRequest)
    {
        if (string.IsNullOrEmpty(variableRequest.ContentId)) throw new PluginMisconfigurationException("The key input is null or empty.");
        if (string.IsNullOrEmpty(variableRequest.CollectionId)) throw new PluginMisconfigurationException("The collection ID input is null or empty.");
        if (string.IsNullOrEmpty(variableRequest.ModeName)) throw new PluginMisconfigurationException("The mode input is null or empty.");

        var fileInfo = await GetFileInfo(variableRequest.ContentId);
        var variablesMeta = await GetFileVariables(variableRequest.ContentId);

        if (!variablesMeta.VariableCollections.TryGetValue(variableRequest.CollectionId, out var collection)) throw new PluginMisconfigurationException($"Cannot find collection with ID '{variableRequest.CollectionId}'");
        var mode = collection.Modes.FirstOrDefault(x => x.Name == variableRequest.ModeName) ?? throw new PluginMisconfigurationException($"Cannot find mode with name '{variableRequest.ModeName}'");

        var variables = variablesMeta.Variables.Values
            .Where(x => x.VariableCollectionId == collection.Id && GetVariableAsString(x, mode) is not null)
            .ToDictionary(x => x.Name, x => GetVariableAsString(x, mode)!);
        var serialized = JsonConvert.SerializeObject(variables, Formatting.Indented);
        var jsonCoder = new JsonCoder();

        var contentName = $"{fileInfo.Name} - {collection.Name}";
        var fileName = contentName + ".json";
        var codedContent = jsonCoder.Deserialize(serialized, fileName);
        codedContent.Language = mode.Name;
        codedContent.OriginalName = contentName;
        codedContent.SystemReference = new SystemReference
        {
            ContentId = $"{variableRequest.ContentId}|{collection.Id}",
            ContentName = $"{fileInfo.Name} - {collection.Name}",
            AdminUrl = $"https://www.figma.com/design/{variableRequest.ContentId}?view=variables",
            SystemName = "Figma",
            SystemRef = "https://www.figma.com"
        };
        codedContent.Provenance.Review.Tool = "Figma";
        codedContent.Provenance.Review.ToolReference = "https://www.figma.com";

        return new FileResponse
        {
            Content = await fileManagementClient.UploadAsync(jsonCoder.ToStream(codedContent), MediaTypeNames.Application.Json, fileName),
        };
    }

    private static string SanitizeCharacters(string input)
    {
        if (input == null)
            return input;

        return input
            .Replace('{', '[')
            .Replace('}', ']')
            .Replace('$', '%')
            .Replace('.', '-');
    }

    [BlueprintActionDefinition(BlueprintAction.UploadContent)]
    [Action("Upload variables", Description = "Upload translated variables to a Figma project")]
    public async Task<UploadVariablesResponse> UploadVariables([ActionParameter] VariableUploadRequest variableRequest)
    {
        if (string.IsNullOrEmpty(variableRequest.ContentId)) throw new PluginMisconfigurationException("The key input is null or empty.");
        if (string.IsNullOrEmpty(variableRequest.CollectionId)) throw new PluginMisconfigurationException("The collection ID input is null or empty.");

        var file = await fileManagementClient.DownloadAsync(variableRequest.Content);
        var transformationResult = Transformation.Load(file, variableRequest.Content.Name, variableRequest.Content.ContentType);
        var contentResult = transformationResult.Target();
        if (!contentResult.Success)
        {
            InvocationContext.Logger?.LogError($"Not a Blackbird interoperable file: {contentResult.Error}", []);
            throw new PluginMisconfigurationException("The file could not be parsed properly, did Blackbird not create this file? See Action logs for more details");
        }

        var fileInfo = await GetFileInfo(variableRequest.ContentId);
        var variablesMeta = await GetFileVariables(variableRequest.ContentId);

        var locale = variableRequest.Locale ?? contentResult.Value.Language;

        if (!variablesMeta.VariableCollections.TryGetValue(variableRequest.CollectionId, out var collection)) throw new PluginMisconfigurationException($"Cannot find collection with ID '{variableRequest.CollectionId}'"); 
        var mode = collection.Modes.FirstOrDefault(x => x.Name == locale);

        CreateModeDto? variableModeChange = null;
        if (mode is null)
        {
            variableModeChange = new CreateModeDto()
            {
                Id = "temp-mode",
                Name = locale,
                VariableCollectionId = collection.Id,
            };
            mode = new Mode()
            {
                ModeId = "temp-mode",
                Name = locale,
            };
        }

        var currentVariables = variablesMeta.Variables.Values
            .Where(x => x.VariableCollectionId == collection.Id);        

        var variableModeValues = new List<VariableModeValueDto>();
        var variablesToBeCreated = new List<CreateVariableDto>();

        foreach (var unit in contentResult.Value.TextUnits)
        {
            var text = unit.GetCodedText();
            if (string.IsNullOrEmpty(text)) continue;
            if (unit.Key is null) continue;

            var name = SanitizeCharacters(unit.Key);
            var currentVariable = currentVariables.FirstOrDefault(x => x.Name == name);
            var currentVariableId = currentVariable?.Id;

            if (currentVariable is null)
            {
                currentVariableId = $"temp-" + variableModeValues.Count;
                variablesToBeCreated.Add(new CreateVariableDto
                {
                    Id = currentVariableId,
                    Name = name,
                    VariableCollectionId = collection.Id,
                });
            } else
            {
                var currentText = GetVariableAsString(currentVariable, mode);
                if (currentText == text) continue;
            }

            variableModeValues.Add(new VariableModeValueDto
            {
                VariableId = currentVariableId!,
                ModeId = mode.ModeId,
                Value = text,
            });
        }

        if (variableModeValues.Count > 0)
        {
            var request = new RestRequest($"/v1/files/{variableRequest.ContentId}/variables", Method.Post);
            request.AddJsonBody(new
            {
                VariableModeValues = variableModeValues,
                Variables = variablesToBeCreated,
                VariableModes = variableModeChange is null ? [] : new List<CreateModeDto>() { variableModeChange }
            });
            await Client.ExecuteWithErrorHandling(request);
        }

        var transformation = transformationResult.Value!;

        transformation.TargetSystemReference = new SystemReference
        {
            ContentId = $"{variableRequest.ContentId}|{collection.Id}",
            ContentName = $"{fileInfo.Name} - {collection.Name}",
            AdminUrl = $"https://www.figma.com/design/{variableRequest.ContentId}?view=variables",
            SystemName = "Figma",
            SystemRef = "https://www.figma.com"
        };
        transformation.TargetLanguage = mode.Name;

        var output = new UploadVariablesResponse
        {
            NumberOfUpdatedVariables = variableModeValues.Count,
            NumberOfNewVariables = variablesToBeCreated.Count
        };

        if (transformationResult.WasBilingual)
        {
            output.Content = await fileManagementClient.UploadAsync(
                transformation.ToStream(),
                MediaTypes.Xliff2,
                transformation.BilingualFileName);
        }
        else
        {
            var targetResult = transformation.Target();
            if (!targetResult.Success)
            {
                output.Content = variableRequest.Content;
                InvocationContext.Logger?.LogError($"Failed to load target file: {targetResult.Error}", []);
            }
            else
            {
                var target = targetResult.Value;
                target.SystemReference = transformation.TargetSystemReference;

                output.Content = await fileManagementClient.UploadAsync(
                    target.ToStream(),
                    target.OriginalMediaType,
                    target.OriginalName);
            }
        }

        return output;
    }
}
