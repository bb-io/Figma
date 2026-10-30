using Blackbird.Applications.Sdk.Common;

namespace Apps.Figma.Models.Responses;

public class SearchPagesResponse
{
    [Display("Pages")]
    public List<PageResponse> Pages { get; set; } = [];
}

public class PageResponse
{
    [Display("Page ID")]
    public string PageId { get; set; } = string.Empty;

    [Display("Page name")]
    public string PageName { get; set; } = string.Empty;
}
