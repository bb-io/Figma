using Apps.Figma.Api;
using Apps.Figma.Constants;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;
using System.Net;

namespace Apps.Figma.Connections;

public class ConnectionValidator(InvocationContext invocationContext) : BaseInvocable(invocationContext), IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        try
        {
            var folderId = authenticationCredentialsProviders.Get(CredsNames.FolderId).Value;
            var client = new Client(authenticationCredentialsProviders);
            var request = new RestRequest($"/v2/folders/{folderId}/files");

            var response = await client.ExecuteAsync(request, cancellationToken);
            var isValid = 
                response.StatusCode != HttpStatusCode.Forbidden && 
                response.StatusCode != HttpStatusCode.NotFound &&
                response.StatusCode != HttpStatusCode.Unauthorized;

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
