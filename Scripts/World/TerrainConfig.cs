using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class TerrainConfig : Resource
{
    [Export(PropertyHint.Range, "-1,1,")]
    public float Min { get; set; } = 0; // min noise value

    [Export(PropertyHint.Range, "-1,1,")]
    public float Max { get; set; } = 0; // max noise value
    [Export]
    public int Layer { get; set; } = 0; // tilemap layer index
}
