using Apps.Figma.Models.Dto;
using Apps.Figma.Models.Requests;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using System.Net;

namespace Apps.Figma.Handlers;
public class VariableCollectionHandler : Invocable, IAsyncDataSourceItemHandler
{
    private readonly string _fileKey;
    public VariableCollectionHandler(InvocationContext invocationContext, [ActionParameter] FileKeyRequest keyRequest) : base(invocationContext)
    {
        _fileKey = keyRequest.ContentId;
    }

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_fileKey)) throw new PluginMisconfigurationException("Please select the 'File key' first.");
        var request = new RestRequest($"/v1/files/{_fileKey}/variables/local", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<VariablesResponseDto>(request);


        var items = response.Meta.VariableCollections.Select(x => new DataSourceItem(x.Key, x.Value.Name)) ?? [];
        if (!string.IsNullOrWhiteSpace(context.SearchString))
            return items.Where(x => x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase));

        return items;
    }
}