using Godot;

namespace Scripts;
public partial class Camera : Camera2D
{
    [Export] public Vector2 TargetedZoom = new(1.5f, 1.5f);
    [Export] public float ZoomSpeed = 0.01f;
    [Export] public Vector2 MinZoom = new(0.5f, 0.5f);
    [Export] public Vector2 MaxZoom = new(2.5f, 2.5f);

    private const float ZOOM_LERP = 15f;
    private const float DRAG_SPEED = 50f;
    private const float SCROLL_SPEED = 25f;

    private Vector2 _defaultZoom = new(1.0f, 1.0f);
    private Vector2 _zoom = new(1.0f, 1.0f);
    private Vector2 _defaultOffset = Vector2.Zero;
    private Vector2 _offset = Vector2.Zero;
    private bool _isDragging = false;

#nullable enable
    private Node2D? _target = null; // target to follow, if any
#nullable disable


    public override void _Ready()
    {
        // store default zoom and offset
        _defaultZoom = Zoom;
        _defaultOffset = Offset;
        _zoom = _defaultZoom;
        _offset = _defaultOffset;
    }

    public override void _Process(double delta)
    {
        if (_zoom != Zoom) // transition zoom smoothly
            Zoom = Zoom.Lerp(_zoom, (float)delta * ZOOM_LERP);

        // move camera
        if (_target is null) // apply offset only
            GlobalPosition = _offset;
        else // follow the target and apply offset
            GlobalPosition = _target.GlobalPosition + _offset;
    }

    public override void _Input(InputEvent @event)
    {
        HandleScroll(@event);
        HandleDrag(@event);
    }

    public void FollowTarget(Node2D target = null)
    {
        if (target is null)
        {
            _zoom = _defaultZoom; // reset zoom if not following a target
            _offset = _target?.GlobalPosition ?? _defaultOffset; // keep camera position
        }
        else
        {
            // update target and reset zoom and offset
            _zoom = TargetedZoom;
            _offset = _defaultOffset;
        }
        _zoom = _zoom.Clamp(MinZoom, MaxZoom);
        _target = target;
    }

    private void HandleScroll(InputEvent @event)
    {
        // handle zoom input
        float zoomDir = 0;
        if (@event is InputEventMagnifyGesture magnifyGesture)
            zoomDir = (magnifyGesture.Factor - 1) * SCROLL_SPEED;
        if (Input.GetAxis("zoom_out", "zoom_in") != 0)
            zoomDir = Input.GetAxis("zoom_out", "zoom_in");

        // zoom in or out
        _zoom += Vector2.One * zoomDir * ZoomSpeed; // apply zoom offset
        _zoom = _zoom.Clamp(MinZoom, MaxZoom); // clamp zoom
    }

    private void HandleDrag(InputEvent @event)
    {
        if (_target is not null)
            return; // do not allow dragging if following a target

        // check dragging gesture
        if (Input.IsActionJustPressed("drag"))
            _isDragging = true;
        if (Input.IsActionJustReleased("drag"))
            _isDragging = false;

        // get offset from input
        var offset = Vector2.Zero;
        if (@event is InputEventPanGesture panGesture)
            offset = panGesture.Delta * DRAG_SPEED;
        if (_isDragging && @event is InputEventMouseMotion mouseMotion)
            offset = -mouseMotion.Relative;

        // offset camera
        _offset += offset;
    }
}
