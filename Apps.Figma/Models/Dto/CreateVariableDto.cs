namespace Apps.Figma.Models.Dto;
public class CreateVariableDto
{
    public string Action { get; set; } = "CREATE";
    public string Name { get; set; }
    public string VariableCollectionId { get; set; }
    public string ResolvedType { get; set; } = "STRING";
    public string Id { get; set; }
}
