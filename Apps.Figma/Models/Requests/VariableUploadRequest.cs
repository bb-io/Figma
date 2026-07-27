using Apps.Figma.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Figma.Models.Requests;
public class VariableUploadRequest : IUploadContentInput
{
    [Display("Variables")]
    public FileReference Content { get; set; }

    [Display("File key", Description = "The key of the figma file. Can be found in the URL: https://www.figma.com/:file_type/:file_key/..."), DataSource(typeof(FileKeyHandler))]
    public string ContentId { get; set; }

    [Display("Collection ID", Description = "The collection that defines your locales")]
    [DataSource(typeof(VariableCollectionHandler))]
    public string CollectionId { get; set; }

    [Display("Target language", Description = "The mode to upload the target variables to")]
    [DataSource(typeof(VariableModeHandler))]
    public string Locale { get; set; }
}
