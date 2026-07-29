using Apps.Figma.Models.Dto;
using Apps.Figma.Models.Requests;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using System.Net;

namespace Apps.Figma.Handlers;
public class VariableModeHandler : Invocable, IAsyncDataSourceItemHandler
{
    private readonly string _fileKey;
    private readonly string _collectionId;
    public VariableModeHandler(InvocationContext invocationContext, [ActionParameter] FileKeyRequest keyRequest, [ActionParameter] VariableDownloadRequest variableRequest) : base(invocationContext)
    {
        _fileKey = keyRequest.ContentId;
        _collectionId = variableRequest.CollectionId;
    }

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_fileKey)) throw new PluginMisconfigurationException("Please select the 'File key' first.");
        if (string.IsNullOrEmpty(_collectionId)) throw new PluginMisconfigurationException("Please select the 'Collection ID' first.");

        var request = new RestRequest($"/v1/files/{_fileKey}/variables/local", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<VariablesResponseDto>(request);

        if (!response.Meta.VariableCollections.TryGetValue(_collectionId, out var collection)) throw new PluginMisconfigurationException($"Cannot find collection with ID '{_collectionId}'");

       
        var items = collection.Modes.Select(x => new DataSourceItem(x.Name, x.Name)) ?? [];
        if (!string.IsNullOrWhiteSpace(context.SearchString))
            return items.Where(x => x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase));

        return items;
    }
}