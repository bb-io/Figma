using Blackbird.Applications.Sdk.Common;

namespace Apps.Figma.Models.Requests;

public class SearchPagesRequest
{
    [Display("Page name contains", Description = "Only return pages whose names contain this text, ignoring case.")]
    public string? PageNameContains { get; set; }
}
