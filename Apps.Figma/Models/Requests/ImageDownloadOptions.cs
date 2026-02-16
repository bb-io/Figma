using Apps.Figma.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Figma.Models.Requests;
public class ImageDownloadOptions
{
    [Display("Format"), StaticDataSource(typeof(ImageFormatHandler))]
    public string? Format { get; set; }
}
