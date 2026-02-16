using Newtonsoft.Json;

namespace Apps.Figma.Models.Dto;
public class ImageMapDto : ErrorDto
{
    [JsonProperty("images")]
    public Dictionary<string, string> ImageMap { get; set; }
}
