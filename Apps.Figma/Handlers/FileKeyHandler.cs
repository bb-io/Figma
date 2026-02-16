using Apps.Figma.Models.Dto;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Figma.Handlers;
public class FileKeyHandler(InvocationContext invocationContext)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new RestRequest($"/v1/projects/{ProjectId}/files", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<FigmaFilesDto>(request);


        var items = response.Files.Select(x => new DataSourceItem(x.Key, x.Name)) ?? [];
        if (!string.IsNullOrWhiteSpace(context.SearchString))
            return items.Where(x => x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase));

        return items;
    }
}
