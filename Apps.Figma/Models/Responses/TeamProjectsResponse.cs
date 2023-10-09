using Apps.Figma.Models.dtos;
using Newtonsoft.Json;

namespace Apps.Figma.Models.Responses;

public class TeamProjectsResponse
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("projects")]
    public List<TeamProject> Projects { get; set; }
}