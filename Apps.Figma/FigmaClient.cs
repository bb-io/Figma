using RestSharp;

namespace Apps.Figma;

public class FigmaClient : RestClient
{
    public FigmaClient() : base(new RestClientOptions() { ThrowOnAnyError = true, BaseUrl = new Uri("https://api.figma.com") }) { }
}