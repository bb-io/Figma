using Newtonsoft.Json;

namespace Apps.Figma.Models.dtos;

public class ProjectFile
{
    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("thumbnail_url")]
    public string ThumbnailUrl { get; set; }

    [JsonProperty("last_modified")]
    public string LastModified { get; set; }
}