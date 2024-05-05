using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class SceneResource : GeneratedResource
{
    [Export]
    public int Source { get; set; } = 0; // tileset source index
    [Export]
    public int Index { get; set; } = 0; // tile index
    [Export]
    public int Layer { get; set; } = 0; // tilemap layer index
}
