using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Figma.Models.Responses;
public class DownloadVariablesResponse : IDownloadContentOutput
{
    [Display("Variables")]
    public FileReference Content { get; set; } = default!;
}
