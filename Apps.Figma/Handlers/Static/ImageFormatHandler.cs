using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Figma.Handlers.Static;
public class ImageFormatHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new ("jpg", "JPG"),
            new ("png", "PNG (Default)"),
            new ("svg", "SVG"),
            new ("pdf", "PDF")
        };
    }
}
