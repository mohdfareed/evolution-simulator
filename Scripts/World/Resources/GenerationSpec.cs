using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class GenerationSpec : Resource
{
    [Export(PropertyHint.Range, "-1,1,")]
    public float Min { get; set; } = 0; // min noise value
    [Export(PropertyHint.Range, "-1,1,")]
    public float Max { get; set; } = 0; // max noise value
    [Export]
    public GeneratedResource? Resource { get; set; } = null; // resource to generate
}
