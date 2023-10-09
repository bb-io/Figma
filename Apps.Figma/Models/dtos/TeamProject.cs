using Newtonsoft.Json;

namespace Apps.Figma.Models.dtos;

public class TeamProject
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}