using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class TileResource : GeneratedResource
{
    [Export]
    public int Source { get; set; } = 0; // tileset source index
    [Export]
    public Vector2I Coordinates { get; set; } = Vector2I.Zero; // tile coordinates
    [Export]
    public int Alternate { get; set; } = 0; // tile alternate index
    [Export]
    public int Layer { get; set; } = 0; // tilemap layer index
}
