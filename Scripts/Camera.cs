using Godot;

namespace Scripts;

/// <summary>
/// 2D camera that follows a target node and allows zooming and panning. It
/// has two modes: following a target and free mode. In free mode, the camera
/// can be dragged around. In target mode, the camera follows the target node.
/// The camera can be zoomed in and out using the mouse wheel or pinch gesture.
/// The camera can be panned by dragging the mouse while holding a button or
/// using a touch pan gesture.
/// </summary>
[Tool]
[GlobalClass]
public partial class Camera : Camera2D
{
    /// <summary>The speed at which the camera pans.</summary>
    [Export] public float PanningSpeed = 1;
    /// <summary>The speed at which the camera zooms in and out.</summary>
    [Export] public float ZoomSpeed = 0.05f;
    /// <summary>The minimum zoom level allowed.</summary>
    [Export] public float MinZoom = 0.1f;
    /// <summary>The maximum zoom level allowed.</summary>
    [Export] public float MaxZoom = 2.0f;

    // constants
    private const float ZOOM_LERP = 10f; // zoom transition speed
    private const float PAN_GESTURE_MODIFIER = 25f; // pan gesture speed
    private const float PINCH_GESTURE_MODIFIER = 10f; // pinch gesture speed

    // state
    private Vector2 _defaultZoom = new(1.0f, 1.0f);
    private Vector2 _zoom = new(1.0f, 1.0f);
    private Vector2 _defaultOffset = Vector2.Zero;
    private Vector2 _offset = Vector2.Zero;
    private Node2D? _target; // target to follow, if any

    public void FollowTarget(Node2D? target)
    {
        if (target is null) // detach from target, keeping position
            _offset = _target?.GlobalPosition ?? _defaultOffset;
        else // reset zoom and offset
            (_offset, _zoom) = (_defaultOffset, _defaultZoom);
        _target = target;
    }

    public override void _Ready()
    {
        // store default zoom and offset
        _defaultZoom = Zoom;
        _defaultOffset = Offset;
        // set initial zoom and offset
        _zoom = _defaultZoom;
        _offset = _defaultOffset;
    }

    public override void _Process(double delta)
    {
        // move camera (node handles transition)
        if (_target is null) // apply offset only
            GlobalPosition = _offset;
        else // follow the target and apply offset
            GlobalPosition = _target.GlobalPosition + _offset;

        // apply smooth zoom
        if (!Zoom.IsEqualApprox(_zoom))
            Zoom = Zoom.Lerp(_zoom, ZOOM_LERP * (float)delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        HandleZoom(@event);
        HandlePan(@event);
        if (Input.IsActionJustPressed("cancel"))
            FollowTarget(null);
    }

    private void HandleZoom(InputEvent @event)
    {
        var zoom = 0f;
        // touch gesture
        if (@event is InputEventMagnifyGesture magnifyGesture)
            zoom = (magnifyGesture.Factor - 1) * PINCH_GESTURE_MODIFIER;
        // keyboard/controller/mouse
        if (Input.GetAxis("zoom_out", "zoom_in") != 0)
            zoom = Input.GetAxis("zoom_out", "zoom_in");

        // zoom in or out, respecting limits
        _zoom += _zoom * ZoomSpeed * zoom;
        _zoom.X = Mathf.Clamp(_zoom.X, MinZoom, MaxZoom);
        _zoom.Y = Mathf.Clamp(_zoom.Y, MinZoom, MaxZoom);
    }

    private void HandlePan(InputEvent @event)
    {
        var pan = Vector2.Zero;
        // touch gestures
        if (@event is InputEventPanGesture panGesture)
            pan = panGesture.Delta * PAN_GESTURE_MODIFIER;
        // mouse drag
        if (Input.IsActionPressed("pan") && @event is InputEventMouseMotion mouseMotion)
            pan = -mouseMotion.Relative; // move in the opposite direction

        if (_target is not null && pan != Vector2.Zero)
            FollowTarget(null); // clear target if panning
        // offset camera relative to zoom level
        _offset += _zoom.Inverse() * PanningSpeed * pan;
    }
}
