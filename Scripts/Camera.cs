using Godot;

namespace Scripts;
public partial class Camera : Camera2D
{
    [Export] public Vector2 TargetedZoom = new(1.5f, 1.5f);
    [Export] public float ZoomSpeed = 0.01f;
    [Export] public float MinZoom = 0.5f;
    [Export] public float MaxZoom = 2.0f;

    private Vector2 _defaultZoom;
    private Vector2 _zoom;
    private Vector2 _defaultOffset;
    private Vector2 _offset;
    private Node2D _target;
    private bool _isDragging = false;


    public override void _Ready()
    {
        // store default zoom and offset
        _defaultZoom = Zoom;
        _defaultOffset = Offset;
        _zoom = Zoom;
        _offset = Offset;
        // set initial target to parent node
        _target = GetParent<Node2D>();
    }

    public override void _Process(double delta)
    {
        // transition to target zoom, offset, and position
        Zoom = Zoom.Lerp(_zoom, (float)delta * 15);
        GlobalPosition = _target.GlobalPosition + _offset;
    }

    public override void _Input(InputEvent @event)
    {
        HandleScroll(@event);
        HandleDrag(@event);
    }

    public void FollowTarget(Node2D target)
    {
        // update target and reset zoom and offset
        _zoom = target is SimulationManager ? _defaultZoom : TargetedZoom;
        _offset = _defaultOffset;
        _target = target;
    }

    private void HandleScroll(InputEvent @event)
    {
        float zoomDir = 0;
        if (@event is InputEventMagnifyGesture magnifyGesture)
            zoomDir = (magnifyGesture.Factor - 1) * 25f;
        if (Input.GetAxis("zoom_out", "zoom_in") != 0)
            zoomDir = Input.GetAxis("zoom_out", "zoom_in");

        // zoom in or out
        var offset = zoomDir * ZoomSpeed; // calculate zoom offset
        _zoom.X = Mathf.Max(MinZoom, Mathf.Min(MaxZoom, _zoom.X + offset));
        _zoom.Y = Mathf.Max(MinZoom, Mathf.Min(MaxZoom, _zoom.Y + offset));
    }

    private void HandleDrag(InputEvent @event)
    {
        if (_target is not SimulationManager)
            return;

        if (@event is InputEventPanGesture panGesture)
            _offset += panGesture.Delta * 50f;

        if (Input.IsActionJustPressed("drag"))
            _isDragging = true;
        if (Input.IsActionJustReleased("drag"))
            _isDragging = false;

        if (_isDragging && @event is InputEventMouseMotion mouseMotion)
            _offset -= mouseMotion.Relative;
    }
}
