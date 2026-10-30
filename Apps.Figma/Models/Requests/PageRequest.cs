using Apps.Figma.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Figma.Models.Requests;

public class PageRequest
{
    [Display("Page ID"), DataSource(typeof(PageHandler))]
    public string? PageId { get; set; }
}
