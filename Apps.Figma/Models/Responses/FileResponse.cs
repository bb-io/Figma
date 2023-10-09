using Newtonsoft.Json;

namespace Apps.Figma.Models.Responses;

public class FileResponse
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("lastModified")]
    public string LastModified { get; set; }

    [JsonProperty("editorType")]
    public string EditorType { get; set; }

    [JsonProperty("thumbnailUrl")]
    public string ThumbnailUrl { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("schemaVersion")]
    public int SchemaVersion { get; set; }

    [JsonProperty("mainFileKey")]
    public string MainFileKey { get; set; }
}