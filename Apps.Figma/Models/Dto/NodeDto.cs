namespace Apps.Figma.Models.Dto;
public class NodeDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public IEnumerable<NodeDto> Children { get; set; }
}

public class DocumentDto
{
    public NodeDto Document { get; set; }
}

public class TopDocumentDto
{
    public string Name { get; set; }
    public Dictionary<string, DocumentDto> Nodes { get; set; }
}