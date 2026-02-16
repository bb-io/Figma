using Apps.Figma.Constants;
using Apps.Figma.Models.Dto;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Figma.Api;

public class Client : BlackBirdRestClient
{
    public Client(IEnumerable<AuthenticationCredentialsProvider> creds) : base(new()
    {
        BaseUrl = new Uri("https://api.figma.com"),
    })
    {
        this.AddDefaultHeader("X-Figma-Token", creds.Get(CredsNames.Token).Value);
    }

    public static string ParseError(RestResponse response)
    {
        if (response.Content != null)
        {
            var error = JsonConvert.DeserializeObject<ErrorDto>(response.Content);
            if (error != null)
            {
                return error.Message;
            }            
        }

        return response.ErrorMessage ?? response.Content ?? string.Empty;
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var errorMessage = ParseError(response);
        throw new PluginApplicationException(errorMessage);
    }
}
