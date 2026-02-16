using Apps.Figma.Api;
using Apps.Figma.Constants;
using Apps.Figma.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Figma.Connections;

public class ConnectionValidator(InvocationContext invocationContext) : BaseInvocable(invocationContext), IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        try
        {
            var projectId = authenticationCredentialsProviders.Get(CredsNames.ProjectId).Value;
            var client = new Client(authenticationCredentialsProviders);
            var request = new RestRequest($"/v1/projects/{projectId}/files");

            var response = await client.ExecuteAsync(request, cancellationToken);

            var isValid = response.StatusCode != System.Net.HttpStatusCode.Forbidden;

            return new ConnectionValidationResponse
            {
                IsValid = isValid,
                Message = isValid ? string.Empty : Client.ParseError(response),
            };

        } 
        catch(Exception ex)
        {
            InvocationContext.Logger?.LogError($"Connection validation failed: {ex.Message}", []);

            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }

    }
}
