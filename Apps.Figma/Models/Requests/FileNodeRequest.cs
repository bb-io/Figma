using Apps.Figma.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Figma.Models.Requests;
public class FileNodeRequest
{
    [Display("Node ID", Description = "Node IDs can be found in the Figma URL: ?node-id=1111-22222. A single node ID always has two numbers separated by a dash."), DataSource(typeof(FileNodeHandler))]
    public string NodeId { get; set; }
}
