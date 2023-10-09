using Blackbird.Applications.Sdk.Common;

namespace Apps.Figma;

public class FigmaApplication : IApplication
{
    public string Name
    {
        get => "Figma";
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}