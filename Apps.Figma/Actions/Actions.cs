using Apps.Figma.Models.Dto;
using Apps.Figma.Models.Requests;
using Apps.Figma.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using System.Net.Mime;

namespace Apps.Figma.Actions;

[ActionList]
public class Actions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Download image", Description = "Download an image from a Figma file")]
    public async Task<ImageResponse> DownloadImage(
        [ActionParameter] FileKeyRequest keyRequest, 
        [ActionParameter] FileNodeRequest nodeRequest, 
        [ActionParameter] ImageDownloadOptions options
        )
    {
        if (string.IsNullOrEmpty(keyRequest.Key)) throw new PluginMisconfigurationException("The key input is null or empty.");
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

        var request = new RestRequest($"/v1/images/{keyRequest.Key}", Method.Get);
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
}
