using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Figma.Models.Responses;
public class ImageResponse
{
    [Display("Node ID")]
    public string NodeId { get; set; }

    [Display("URL")]
    public string Url { get; set; }

    [Display("Image file")]
    public FileReference File { get; set; }
}
