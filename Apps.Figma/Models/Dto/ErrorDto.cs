using Newtonsoft.Json;

namespace Apps.Figma.Models.Dto;
public class ErrorDto
{
    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("err")]
    public string Err { get; set; }

    [JsonProperty("error")]
    public string Error { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}
