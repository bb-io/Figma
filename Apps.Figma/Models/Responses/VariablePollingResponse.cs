using Apps.Figma.Models.Requests;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
namespace Apps.Figma.Models.Responses;
public class VariablePollingResponse : IMultiDownloadableContentOutput<VariableDownloadRequest>
{
    public List<VariableDownloadRequest> Items { get;  set; }

    [Display("Default mode", Description = "The mode defined as default in Figma, often the source language")]
    public string? DefaultMode { get; set; }

    [Display("Other modes", Description = "The other modes defined for these variables in Figma")]
    public IEnumerable<string> OtherModes { get; set; } = [];
}
