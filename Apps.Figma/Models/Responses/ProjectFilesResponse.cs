using Apps.Figma.Models.dtos;
using Newtonsoft.Json;

namespace Apps.Figma.Models.Responses;

public class ProjectFilesResponse
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("files")]
    public List<ProjectFile> Files { get; set; }
}