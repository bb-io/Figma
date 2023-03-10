using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Apps.OpenAI.Models.Responses;
using Apps.Zendesk.Models.Requests;
using RestSharp;
using System.Buffers.Text;
using System.Text;
using Apps.Figma.Models.Responses;
using Newtonsoft.Json;
using Apps.Figma.Models.Requests;

namespace Apps.Zendesk.Actions
{
    [ActionList]
    public class Actions
    {
        [Action]
        public FileResponse GetFileByKey(AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] FileRequest input)
        {            
            var fileEndpoint = $"/v1/files/{input.Key}";
            var request = CreateRequestToFigma(authenticationCredentialsProvider.Value, fileEndpoint, Method.Get);
            var response = new RestClient("https://api.figma.com").Get(request);

            dynamic fileResponse = JsonConvert.DeserializeObject(response.Content);
            string title = fileResponse.name;
            string lastModified = fileResponse.lastModified;
            return new FileResponse()
            {
                FileName = title,
                LastModified = lastModified
            };
        }

        [Action]
        public TeamProjectsResponse GetTeamProjects(AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] TeamProjectsRequest input)
        {
            var teamProjectsEndpoint = $"/v1/teams/{input.TeamId}/projects";
            var request = CreateRequestToFigma(authenticationCredentialsProvider.Value, teamProjectsEndpoint, Method.Get);
            var response = new RestClient("https://api.figma.com").Get(request);

            return new TeamProjectsResponse()
            {
                TeamProjects = response.Content
            };
        }

        [Action]
        public ProjectFilesResponse GetProjectFiles(AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] ProjectFilesRequest input)
        {
            var teamProjectsEndpoint = $"/v1/projects/{input.ProjectId}/files";
            var request = CreateRequestToFigma(authenticationCredentialsProvider.Value, teamProjectsEndpoint, Method.Get);
            var response = new RestClient("https://api.figma.com").Get(request);

            return new ProjectFilesResponse()
            {
                ProjectFiles = response.Content
            };
        }

        private RestRequest CreateRequestToFigma(string token, string endpoint,
            RestSharp.Method methodType)
        {
            var request = new RestRequest(endpoint, methodType);
            request.AddHeader("X-Figma-Token", token);
            return request;
        }
    }
}
