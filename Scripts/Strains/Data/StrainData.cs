using Godot;

namespace HempAdventures.Scripts.Strains.Data;

public partial class StrainData : Resource
{
    public string Name { get; set; }
    public StrainData() { }
    public StrainData(string name)
    {
        Name = name;
    }
}