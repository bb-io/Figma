using Apps.Figma.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Figma.Models.Requests;
public class FileKeyRequest : IDownloadContentInput
{
    [Display("File key", Description = "The key of the figma file. Can be found in the URL: https://www.figma.com/:file_type/:file_key/..."), DataSource(typeof(FileKeyHandler))]
    public string ContentId { get; set; }
}
