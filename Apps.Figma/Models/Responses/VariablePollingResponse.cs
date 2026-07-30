using Apps.Figma.Models.Requests;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
namespace Apps.Figma.Models.Responses;
public class VariablePollingResponse : IMultiDownloadableContentOutput<VariableDownloadRequest>
{
    public List<VariableDownloadRequest> Items { get;  set; }
}
