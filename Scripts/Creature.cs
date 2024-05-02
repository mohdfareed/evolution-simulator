using Godot;

namespace Scripts;
public partial class Creature : CharacterBody2D
{
    private float _speed = 250;


    public override void _Process(double delta)
    {
        Velocity = Velocity.Lerp(CalculateVelocity(), 0.2f);
        Rotate(Vector2.Zero.DirectionTo(Velocity).Angle() - GlobalRotation + Mathf.Pi / 2);
        MoveAndSlide();
    }

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
        if (Input.IsActionJustPressed("ui_select"))
        {
            SimulationManager.Instance.MainCamera.FollowTarget(this);
        }
    }

    public Vector2 CalculateVelocity()
    {
        Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        return inputDir.Normalized() * _speed;
    }
}
