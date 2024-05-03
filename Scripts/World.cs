using Godot;

namespace Scripts;
public partial class World : StaticBody2D
{
    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
        if (Input.IsActionJustPressed("ui_select"))
        {
            SimulationManager.Instance.MainCamera.FollowTarget(null); // clear camera target
        }
    }
}
