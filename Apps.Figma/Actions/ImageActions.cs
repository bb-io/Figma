using Apps.Figma.Models.Dto;
using Apps.Figma.Models.Requests;
using Apps.Figma.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Figma.Actions;

[ActionList("Images")]
public class ImageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    [Action("Download image", Description = "Download an image from a file")]
    public async Task<ImageResponse> DownloadImage(
        [ActionParameter] FileKeyRequest keyRequest, 
        [ActionParameter] FileNodeRequest nodeRequest, 
        [ActionParameter] ImageDownloadOptions options
        )
    {
        if (string.IsNullOrWhiteSpace(keyRequest.ContentId)) throw new PluginMisconfigurationException("File key is required.");
        if (string.IsNullOrEmpty(nodeRequest.NodeId)) throw new PluginMisconfigurationException("The node ID input is null or empty.");

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
            File = new FileReference(new HttpRequestMessage(HttpMethod.Get, image.Value), $"{imageName}.{extension}", MimeTypes.GetMimeType($".{extension}")),
        };
    }

    [Action("Download images from frames", Description = "Download images of top-level frames from a file or page")]
    public async Task<FrameImagesResponse> DownloadImagesFromFrames(
        [ActionParameter] FileKeyRequest keyRequest,
        [ActionParameter] PageRequest pageRequest,
        [ActionParameter] ImageDownloadOptions options)
    {
        if (string.IsNullOrWhiteSpace(keyRequest.ContentId))
            throw new PluginMisconfigurationException("File key is required.");

        var format = options.Format ?? "png";

        var fileRequest = new RestRequest($"/v1/files/{keyRequest.ContentId}", Method.Get);
        fileRequest.AddQueryParameter("depth", 2);
        var file = await Client.ExecuteWithErrorHandling<DocumentDto>(fileRequest);
        if (file.Document?.Children == null)
            throw new PluginApplicationException("Figma did not return the file's pages.");
        var pages = file.Document.Children.Where(x => x.Type == "CANVAS").ToList();
        if (!string.IsNullOrWhiteSpace(pageRequest.PageId))
        {
            pages = pages.Where(x => x.Id == pageRequest.PageId).ToList();
            if (pages.Count == 0)
                throw new PluginMisconfigurationException($"Page '{pageRequest.PageId}' was not found in the selected file.");
        }

        var frames = pages.SelectMany(page => (page.Children ?? [])
            .Where(node => node.Type == "FRAME")
            .Select(node => (Page: page, Frame: node))).ToList();
        var images = new List<FrameImageResponse>();
        foreach (var batch in frames.Chunk(10))
        {
            var ids = batch.Select(x => x.Frame.Id).ToArray();
            var request = new RestRequest($"/v1/images/{keyRequest.ContentId}", Method.Get);
            request.AddQueryParameter("ids", string.Join(",", ids));
            request.AddQueryParameter("format", format);
            ImageMapDto response;
            try
            {
                response = await Client.ExecuteWithErrorHandling<ImageMapDto>(request);
            }
            catch (PluginApplicationException ex)
            {
                throw new PluginApplicationException($"Failed to render frames {string.Join(", ", ids)}: {ex.Message}");
            }

            if (!string.IsNullOrWhiteSpace(response.Err) || !string.IsNullOrWhiteSpace(response.Error))
                throw new PluginApplicationException($"Failed to render frames {string.Join(", ", ids)}: {(string.IsNullOrWhiteSpace(response.Err) ? response.Error : response.Err)}");

            var failedIds = ids.Where(id => response.ImageMap == null ||
                !response.ImageMap.TryGetValue(id, out var url) || string.IsNullOrWhiteSpace(url)).ToArray();
            if (failedIds.Length > 0)
                throw new PluginApplicationException($"Figma could not render frames: {string.Join(", ", failedIds)}.");

            foreach (var item in batch)
            {
                var url = response.ImageMap![item.Frame.Id];
                var invalidCharacters = Path.GetInvalidFileNameChars().Concat("<>:\"/\\|?*".ToCharArray()).ToHashSet();
                var name = new string((item.Frame.Name ?? "frame").Select(c => invalidCharacters.Contains(c) || char.IsControl(c) ? '_' : c).ToArray()).Trim().TrimEnd('.');
                if (string.IsNullOrWhiteSpace(name)) name = "frame";
                var safeId = new string(item.Frame.Id.Select(c => invalidCharacters.Contains(c) || char.IsControl(c) ? '_' : c).ToArray());
                images.Add(new FrameImageResponse
                {
                    NodeId = item.Frame.Id,
                    FrameName = item.Frame.Name ?? "frame",
                    PageId = item.Page.Id,
                    PageName = item.Page.Name,
                    Url = url,
                    File = new FileReference(new HttpRequestMessage(HttpMethod.Get, url), $"{name}_{safeId}.{format}", MimeTypes.GetMimeType($".{format}"))
                });
            }
        }

        return new FrameImagesResponse { Images = images };
    }
}
