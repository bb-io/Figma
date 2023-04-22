using Blackbird.Applications.Sdk.Common.Authentication;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Figma
{
    public class FigmaRequest : RestRequest
    {
        public FigmaRequest(string endpoint, Method method, IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) : base (endpoint, method) 
        {
            var token = authenticationCredentialsProviders.First(p => p.KeyName == "apiToken").Value;
            this.AddHeader("X-Figma-Token", token);
        }
    }
}
