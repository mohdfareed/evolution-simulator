using Godot;

namespace Scripts;
public partial class World : StaticBody2D
{
    Node _chunkLoader;


    public override void _Ready()
    {
        _chunkLoader = GetNode("ChunkLoader");
    }

    public void LoadChunk(Node2D actor)
    {
        _chunkLoader.Set("actor", actor);
        _chunkLoader.Call("_process", 0f);
    }

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
        if (Input.IsActionJustPressed("ui_select"))
        {
            SimulationManager.Instance.MainCamera.FollowTarget(null); // clear camera target
        }
    }
}
