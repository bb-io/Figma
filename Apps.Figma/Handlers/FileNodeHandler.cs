using Apps.Figma.Models.Dto;
using Apps.Figma.Models.Requests;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Figma.Handlers;
public class FileNodeHandler : Invocable, IAsyncDataSourceItemHandler
{
    private readonly string _fileKey;
    public FileNodeHandler(InvocationContext invocationContext, [ActionParameter] FileKeyRequest keyRequest) : base (invocationContext)
    {
        _fileKey = keyRequest.Key;
    }

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_fileKey)) throw new PluginMisconfigurationException("Please select the 'File key' first.");
        var request = new RestRequest($"/v1/files/{_fileKey}/nodes", Method.Get);

        request.AddQueryParameter("ids", "0-0");
        request.AddQueryParameter("depth", 1);
        var response = await Client.ExecuteWithErrorHandling<TopDocumentDto>(request);


        var items = response.Nodes.FirstOrDefault().Value.Document.Children.Select(x => new DataSourceItem(x.Id, x.Name)) ?? [];
        if (!string.IsNullOrWhiteSpace(context.SearchString))
            return items.Where(x => x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase));

        return items;
    }
}
