using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Apps.Figma.Models.Requests;
using RestSharp;
using Apps.Figma.Models.Responses;
using Blackbird.Applications.Sdk.Common.Actions;

namespace Apps.Figma.Actions;

[ActionList]
public class Actions
{
    [Action("Get file", Description = "Get file information on a specific file")]
    public FileResponse GetFileByKey(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] FileRequest input)
    {

        var client = new FigmaClient();
        var request = new FigmaRequest($"/v1/files/{input.Key}", Method.Get, authenticationCredentialsProviders);
        return client.Get<FileResponse>(request);
    }

    [Action("Get team projects", Description = "Get all the projects of a specific team")]
    public TeamProjectsResponse GetTeamProjects(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] TeamProjectsRequest input)
    {

        var client = new FigmaClient();
        var request = new FigmaRequest($"/v1/teams/{input.TeamId}/projects", Method.Get, authenticationCredentialsProviders);
        return client.Get<TeamProjectsResponse>(request);
    }

    [Action("Get project files", Description = "Get all the files of a specific project")]
    public ProjectFilesResponse GetProjectFiles(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] ProjectFilesRequest input)
    {
        var client = new FigmaClient();
        var request = new FigmaRequest($"/v1/projects/{input.ProjectId}/files", Method.Get, authenticationCredentialsProviders);
        return client.Get<ProjectFilesResponse>(request);
    }
}