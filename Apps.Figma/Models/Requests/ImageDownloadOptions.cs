using Apps.Figma.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Figma.Models.Requests;
public class ImageDownloadOptions
{
    [Display("Format", Description = "Image format. If omitted, PNG is used."), StaticDataSource(typeof(ImageFormatHandler))]
    public string? Format { get; set; }
}
