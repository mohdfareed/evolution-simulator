using Godot;

namespace Scripts;

/// <summary>
/// 2D camera that follows a target node and allows zooming and dragging. It
/// has two modes: following a target and free mode. In free mode, the camera
/// can be dragged around. In target mode, the camera follows the target node.
/// The camera can be zoomed in and out using the mouse wheel or pinch gesture.
/// </summary>
public partial class Camera : Camera2D
{
    /// <summary>The zoom level when following a target node.</summary>
    [Export] public Vector2 TargetingZoom = new(1.5f, 1.5f);
    /// <summary>The minimum zoom level allowed.</summary>
    [Export] public Vector2 MinZoom = new(0.5f, 0.5f);
    /// <summary>The maximum zoom level allowed.</summary>
    [Export] public Vector2 MaxZoom = new(3.5f, 3.5f);

    private const float DRAG_SPEED = 25; // dragging speed
    private const float SCROLL_SPEED = 0.01f; // mouse scroll zoom speed
    private const float ZOOM_LERP = 5f; // zoom transition speed
    private readonly Vector2 _maxZoomSpeed = new(2.5f, 2.5f); // max change in zoom

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
        // set initial zoom and offset
        _zoom = _defaultZoom;
        _offset = _defaultOffset;
    }

    public override void _Process(double delta)
    {
        // move camera (component handles transitions)
        if (_target is null) // apply offset only
            GlobalPosition = _offset;
        else // follow the target and apply offset
            GlobalPosition = _target.GlobalPosition + _offset;

        // apply zoom
        if (_zoom != Zoom)
        {
            var newZoom = Zoom.Lerp(_zoom, ZOOM_LERP); // smooth transition
            // limit the speed at which the camera zooms in and out
            Zoom += (newZoom - Zoom).Clamp(-_maxZoomSpeed, _maxZoomSpeed) * (float)delta;
        }
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
            // reset zoom and offset
            _zoom = TargetingZoom;
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
            zoomDir = magnifyGesture.Factor - 1;
        if (Input.GetAxis("zoom_out", "zoom_in") != 0)
            zoomDir = Input.GetAxis("zoom_out", "zoom_in") * SCROLL_SPEED;

        // zoom in or out
        _zoom += Vector2.One * zoomDir; // apply zoom offset
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
            offset = -mouseMotion.Relative; // move in the opposite direction

        // offset camera
        _offset += offset;
    }
}
