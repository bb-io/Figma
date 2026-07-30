using Apps.Figma.Api;
using Apps.Figma.Constants;
using Apps.Figma.Models.Dto;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace Apps.Figma;

public class Invocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected string ProjectId { get; set; }

    protected Client Client { get; }
    public Invocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new(Creds);
        ProjectId = Creds.Get(CredsNames.ProjectId).Value;
    }

    protected async Task<Meta> GetFileVariables(string key)
    {
        var request = new RestRequest($"/v1/files/{key}/variables/local", Method.Get);
        var response = await Client.ExecuteWithErrorHandling<VariablesResponseDto>(request);
        return response.Meta;
    }

    protected string? GetVariableAsString(Variable variable, Mode mode)
    {
        variable.ValuesByMode.TryGetValue(mode.ModeId, out var valueByNode);
        if (valueByNode is not string && valueByNode is not null) return null;
        return (string?)valueByNode;
    }
}
