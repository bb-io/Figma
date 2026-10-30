using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Figma.Models.Responses;

public class FrameImagesResponse
{
    [Display("Images")]
    public IEnumerable<FrameImageResponse> Images { get; set; } = [];
}

public class FrameImageResponse
{
    [Display("Frame ID")]
    public string NodeId { get; set; } = string.Empty;

    [Display("Frame name")]
    public string FrameName { get; set; } = string.Empty;

    [Display("Page ID")]
    public string PageId { get; set; } = string.Empty;

    [Display("Page name")]
    public string PageName { get; set; } = string.Empty;

    [Display("URL")]
    public string Url { get; set; } = string.Empty;

    [Display("Image file")]
    public FileReference File { get; set; } = null!;
}
