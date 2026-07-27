using Apps.Figma.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Figma.Models.Requests;
public class VariableDownloadRequest
{
    [Display("Collection ID", Description = "The collection that defines your locales")]
    [DataSource(typeof(VariableCollectionHandler))]
    public string CollectionId { get; set; }

    [Display("Target language", Description = "The mode to download as the target language")]
    [DataSource(typeof(VariableModeHandler))]
    public string TargetMode { get; set; }

    [Display("Source language", Description = "The mode to download as the source language, by default the default mode is used")]
    [DataSource(typeof(VariableModeHandler))]
    public string? SourceMode { get; set; }
}
