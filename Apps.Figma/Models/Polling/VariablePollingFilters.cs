using Apps.Figma.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Figma.Models.Polling;
public class VariablePollingFilters
{
    [Display("File key", Description = "The key of the figma file. Can be found in the URL: https://www.figma.com/:file_type/:file_key/..."), DataSource(typeof(FileKeyHandler))]
    public string ContentId { get; set; }

    [Display("Collection ID", Description = "The collection that defines your locales")]
    [DataSource(typeof(VariableCollectionHandler))]
    public string CollectionId { get; set; }

    [Display("Modes", Description = "The modes to consider. By default all modes are polled.")]
    [DataSource(typeof(VariableModeHandler))]
    public IEnumerable<string>? ModeNames { get; set; }
}
