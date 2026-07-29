using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Figma.Models.Dto;
public class CreateModeDto
{
    public string Action { get; set; } = "CREATE";
    public string Name { get; set; }
    public string VariableCollectionId { get; set; }
    public string Id { get; set; }
}
