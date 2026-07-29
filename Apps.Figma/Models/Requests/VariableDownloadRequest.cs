using Apps.Figma.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Figma.Models.Requests;
public class VariableDownloadRequest
{

    [Display("Collection ID", Description = "The collection that defines your locales")]
    [DataSource(typeof(VariableCollectionHandler))]
    public string CollectionId { get; set; }

    [Display("Mode", Description = "The mode to download")]
    [DataSource(typeof(VariableModeHandler))]
    public string ModeName { get; set; }
}
