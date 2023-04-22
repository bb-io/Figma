using Apps.Figma.Models.dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Figma.Models.Responses
{
    public class ProjectFilesResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("files")]
        public List<ProjectFile> Files { get; set; }
    }
}
