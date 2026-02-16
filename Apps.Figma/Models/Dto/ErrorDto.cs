using Newtonsoft.Json;

namespace Apps.Figma.Models.Dto;
public class ErrorDto
{
    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("err")]
    public string Message { get; set; }
}
