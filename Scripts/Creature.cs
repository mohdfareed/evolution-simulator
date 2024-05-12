using Godot;

namespace Scripts;
[Tool]
[GlobalClass]
public partial class Creature : CharacterBody2D
{
    [Export] public float Speed { get; set; } = 150f;
    [Signal] public delegate void SelectedEventHandler(Creature creature);

    public const float ACCELERATION = 1.5f;
    public const float STOPPING_DISTANCE = 0.1f;

    private Vector2 _velocity = Vector2.Zero;
    private Timer _movementTimer = new();

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
        if (Input.IsActionJustPressed("select"))
            EmitSignal(SignalName.Selected, this);
    }

    public override void _Ready()
    {
        _movementTimer.WaitTime = 1;
        _movementTimer.Autostart = true;
        // _movementTimer.Timeout += MoveRandomly;
        AddChild(_movementTimer);
    }

    public override void _PhysicsProcess(double delta)
    {
        // stop the creature if it's close to stopping distance
        if (_velocity.Length() < STOPPING_DISTANCE)
            _velocity = Vector2.Zero;

        // move and rotate the creature in the direction of the velocity
        Velocity = Velocity.Lerp(_velocity, ACCELERATION * (float)delta);
        Rotate(Vector2.Zero.DirectionTo(Velocity).Angle() - GlobalRotation + Mathf.Pi / 2);
        MoveAndSlide();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // get input vector
        var input = Input.GetVector("manual_left", "manual_right", "manual_up", "manual_down");
        _velocity = input.Normalized() * Speed;
    }

    private void MoveRandomly()
    {
        Vector2 randDir = new(GD.RandRange(-1, 1), GD.RandRange(-1, 1));
        _velocity = randDir.Normalized() * Speed;
    }
}
