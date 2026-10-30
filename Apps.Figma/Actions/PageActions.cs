using Apps.Figma.Models.Dto;
using Apps.Figma.Models.Requests;
using Apps.Figma.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Figma.Actions;

[ActionList("Pages")]
public class PageActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Search pages in a file", Description = "Find pages in a Figma file")]
    public async Task<SearchPagesResponse> SearchPages([ActionParameter] FileKeyRequest keyRequest, [ActionParameter] SearchPagesRequest searchRequest)
    {
        if (string.IsNullOrWhiteSpace(keyRequest.ContentId))
            throw new PluginMisconfigurationException("Please select the 'File key' first.");

        var request = new RestRequest($"/v1/files/{keyRequest.ContentId}", Method.Get);
        request.AddQueryParameter("depth", 1);
        var file = await Client.ExecuteWithErrorHandling<DocumentDto>(request);
        if (file?.Document?.Children == null)
            throw new PluginApplicationException("Figma did not return the file's pages.");

        return new SearchPagesResponse
        {
            Pages = file.Document.Children
                .Where(node => node.Type == "CANVAS")
                .Where(node => string.IsNullOrWhiteSpace(searchRequest.PageNameContains) ||
                    (node.Name?.Contains(searchRequest.PageNameContains, StringComparison.OrdinalIgnoreCase) ?? false))
                .Select(node => new PageResponse { PageId = node.Id, PageName = node.Name })
                .ToList()
        };
    }
}
