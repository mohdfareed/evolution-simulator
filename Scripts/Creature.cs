using Godot;

namespace Scripts;
public partial class Creature : CharacterBody2D
{
    public const float ACCELERATION = 0.15f;
    public const float STOPPING_DISTANCE = 0.1f;

    public float Speed { get; set; } = 1.5f;

    private Vector2 _velocity = Vector2.Zero;
    private float _size = 32f;
    private Color _color = new(1f, 0.25f, 0.1f);
    private Color _accentColor = new(0.1f, 0.35f, 1f);


    public override void _PhysicsProcess(double delta)
    {
        // stop the creature if it's close to stopping distance
        if (_velocity.Length() < STOPPING_DISTANCE)
            _velocity = Vector2.Zero;

        // move and rotate the creature in the direction of the velocity
        Velocity = Velocity.Lerp(_velocity, ACCELERATION);
        Rotate(Vector2.Zero.DirectionTo(Velocity).Angle() - GlobalRotation + Mathf.Pi / 2);
        MoveAndSlide();
        SimulationManager.Instance.GameWorld.LoadChunk(this);
    }

    public override void _Input(InputEvent @event)
    {
        Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        _velocity = inputDir.Normalized() * Speed;
        _velocity *= SimulationManager.Instance.PixelsPerMeter; // convert to pixels per second
    }

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
        if (Input.IsActionJustPressed("ui_select"))
        {
            SimulationManager.Instance.MainCamera.FollowTarget(this);
        }
    }

    public override void _Draw()
    {
        // Draw the creature as a circle with a triangle pointing in the direction of movement
        DrawCircle(Vector2.Zero, _size, _color);
        DrawColoredPolygon(new Vector2[]
        {
            new (0, -_size * 0.75f),
            new (-_size * 0.25f, _size * 0.25f),
            new (_size * 0.25f, _size * 0.25f)
        }, _accentColor);
    }
}
