using Apps.Figma.Models.Polling;
using Apps.Figma.Models.Requests;
using Apps.Figma.Models.Responses;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Blackbird.Applications.SDK.Blueprints;
using System.Security.Cryptography;
using System.Text;

namespace Apps.Figma.Events;

[PollingEventList("Variables")]
public class VariablePollingList(InvocationContext invocationContext) : Invocable(invocationContext)
{
    private string CreateHash(Dictionary<string, string> variables)
    {
        var sb = new StringBuilder();

        foreach (var kvp in variables.OrderBy(x => x.Key, StringComparer.Ordinal))
        {
            sb.Append(kvp.Key)
              .Append('=')
              .Append(kvp.Value)
              .Append(';');
        }

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()));
        return Convert.ToHexString(bytes);
    }


    [BlueprintEventDefinition(BlueprintEvent.ContentCreatedOrUpdatedMultiple)]
    [PollingEvent("On variables updated",
       Description = "Triggered on an interval and outputs the variables of the mode updated.")]
    public async Task<PollingEventResponse<VariablesPollingMemory, VariablePollingResponse>> OnVariablesUpdated(
       PollingEventRequest<VariablesPollingMemory> request,
       [PollingEventParameter] VariablePollingFilters filter)
    {
        if (string.IsNullOrEmpty(filter.ContentId)) throw new PluginMisconfigurationException("The key input is null or empty.");
        if (string.IsNullOrEmpty(filter.CollectionId)) throw new PluginMisconfigurationException("The collection ID input is null or empty.");

        var variablesMeta = await GetFileVariables(filter.ContentId);

        if (!variablesMeta.VariableCollections.TryGetValue(filter.CollectionId, out var collection)) throw new PluginMisconfigurationException($"Cannot find collection with ID '{filter.CollectionId}'");

        var variablesByMode = new Dictionary<string, Dictionary<string, string>>();

        foreach(var mode in collection.Modes)
        {
            var variables = variablesMeta.Variables.Values
                .Where(x => x.VariableCollectionId == collection.Id && GetVariableAsString(x, mode) is not null)
                .ToDictionary(x => x.Name, x => GetVariableAsString(x, mode)!);

            variablesByMode.Add(mode.Name, variables);
        }

        var variableHashesByMode = variablesByMode.ToDictionary(x => x.Key, x => CreateHash(x.Value));

        if (request.Memory?.VariablesHashByMode is null)
            return new()
            {
                FlyBird = false,
                Memory = new VariablesPollingMemory { VariablesHashByMode = variableHashesByMode },
                Result = null
            };

        var modesToConsider = filter.ModeNames ?? collection.Modes.Select(x => x.Name) ?? [];
        List<VariableDownloadRequest> items = [];

        foreach (var modeName in modesToConsider) 
        {
            if (!variableHashesByMode.TryGetValue(modeName, out var currentVariableHash)) continue;
            if (!request.Memory.VariablesHashByMode.TryGetValue(modeName, out var memoryVariableHash)) continue;
            if (currentVariableHash == memoryVariableHash) continue;

            items.Add(new VariableDownloadRequest
            {
                CollectionId = filter.CollectionId,
                ContentId = filter.ContentId,
                ModeName = modeName,
            });
        }

        return new PollingEventResponse<VariablesPollingMemory, VariablePollingResponse>
        {
            FlyBird = items.Count > 0,
            Memory = new VariablesPollingMemory { VariablesHashByMode = variableHashesByMode },
            Result = items.Count > 0
                ? new VariablePollingResponse { Items = items }
                : null
        };
    }
}
