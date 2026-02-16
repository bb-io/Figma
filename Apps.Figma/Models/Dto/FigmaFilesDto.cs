namespace Apps.Figma.Models.Dto;
public class FigmaFilesDto
{
    public IEnumerable<FigmaFileDto> Files { get; set; }
}

public class FigmaFileDto
{
    public string Key { get; set; }
    public string Name { get; set; }
}
