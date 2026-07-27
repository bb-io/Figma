using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Figma.Models.Responses;
public class UploadVariablesResponse : IDownloadContentOutput
{
    [Display("Variables")]
    public FileReference Content { get; set; } = default!;

    [Display("Updated variables", Description = "The number of variables updated")]
    public int NumberOfUpdatedVariables { get; set; } = 0;
}
