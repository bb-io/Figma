using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Figma
{
    public class FigmaClient : RestClient
    {
        public FigmaClient() : base(new RestClientOptions() { ThrowOnAnyError = true, BaseUrl = new Uri("https://api.figma.com") }) { }
    }
}
