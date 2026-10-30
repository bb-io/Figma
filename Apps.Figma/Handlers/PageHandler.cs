using Apps.Figma.Models.Dto;
using Apps.Figma.Models.Requests;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Figma.Handlers;

public class PageHandler(InvocationContext invocationContext, [ActionParameter] FileKeyRequest keyRequest)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(keyRequest.ContentId))
            throw new PluginMisconfigurationException("Please select the 'File key' first.");

        var request = new RestRequest($"/v1/files/{keyRequest.ContentId}", Method.Get);
        request.AddQueryParameter("depth", 1);
        var response = await Client.ExecuteWithErrorHandling<DocumentDto>(request);
        if (response.Document?.Children == null)
            throw new PluginApplicationException("Figma did not return the file's pages.");
        var pages = response.Document.Children
            .Where(x => x.Type == "CANVAS")
            .Select(x => new DataSourceItem(x.Id, x.Name));
        return string.IsNullOrWhiteSpace(context.SearchString)
            ? pages
            : pages.Where(x => x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase));
    }
}
