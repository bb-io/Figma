using Apps.Figma.Models.Requests;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Figma.Models.Responses;

public class VariableUpdatedResponse : VariableDownloadRequest
{
    [Display("Default mode", Description = "The mode defined as default in Figma, often the source language")]
    public string DefaultMode { get; set; } = string.Empty;

    [Display("Other modes", Description = "The other modes defined for these variables in Figma")]
    public IEnumerable<string> OtherModes { get; set; } = [];
}
