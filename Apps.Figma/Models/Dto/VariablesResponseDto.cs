namespace Apps.Figma.Models.Dto;
public class VariablesResponseDto : ErrorDto
{
    public Meta Meta { get; set; }
}

public class Meta
{
    public Dictionary<string, Variable> Variables { get; set; }
    public Dictionary<string, VariableCollection> VariableCollections { get; set; }
}

public class Variable
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Key { get; set; }
    public string VariableCollectionId { get; set; }
    public string ResolvedType { get; set; } // BOOLEAN, FLOAT, STRING, COLOR
    public Dictionary<string, object> ValuesByMode { get; set; }
    public bool Remote { get; set; }
    public string Description { get; set; }
    public bool HiddenFromPublishing { get; set; }

    // ...Scopes
    // ...CodeSyntax
}

public class VariableCollection
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Key { get; set; }
    public List<Mode> Modes { get; set; }
    public string DefaultModeId { get; set; }
    public bool Remote { get; set; }
    public bool HiddenFromPublishing { get; set; }
    public List<string> VariableIds { get; set; }
    public bool IsExtension { get; set; }
    public string? ParentVariableCollectionId { get; set; }
    public string? RootVariableCollectionId { get; set; }
    public List<string>? InheritedVariableIds { get; set; }
    public List<string>? LocalVariableIds { get; set; }

    // ...VariableOverrides

    public bool DeletedButReferenced { get; set; }


}

public class Mode
{
    public string ModeId { get; set; }
    public string Name { get; set; }
    public string? ParentModeId { get; set; }
}
