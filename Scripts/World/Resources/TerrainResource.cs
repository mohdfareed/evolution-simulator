using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class TerrainResource : GeneratedResource
{
    [Export]
    public int TerrainSet { get; set; } = 0; // tileset terrain index
    [Export]
    public int Index { get; set; } = 0; // terrain index
    [Export]
    public int Layer { get; set; } = 0; // tilemap layer index
}
