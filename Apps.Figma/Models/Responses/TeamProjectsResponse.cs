using Apps.Figma.Models.dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Figma.Models.Responses
{
    public class TeamProjectsResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("projects")]
        public List<TeamProject> Projects { get; set; }
    }
}
