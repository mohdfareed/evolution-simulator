using Godot;

namespace Scripts;
public partial class Creature : CharacterBody2D
{
    [Export]
    public float Acceleration = 15f;

    private float _speed = 1.5f;

    private Vector2 _velocity = Vector2.Zero;


    public override void _Process(double delta)
    {
        var velocity = _velocity * SimulationManager.Instance.PixelsPerMeter;
        Velocity = Velocity.Lerp(velocity, (float)delta * Acceleration);
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

    public override void _Input(InputEvent @event)
    {
        Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        _velocity = inputDir.Normalized() * _speed;
    }
}
