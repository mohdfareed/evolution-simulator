using Godot;

namespace Scripts;
public partial class SimulationManager : Node2D
{
    public Camera MainCamera { get; private set; }

    public static SimulationManager Instance { get; private set; }


    public SimulationManager()
    {
        if (Instance != null)
        {
            GD.PrintErr("SimulationManager is a singleton and cannot have more than one instance.");
            return;
        }
        Instance = this;
    }

    public override void _Ready()
    {
        Instance.MainCamera = GetNode<Camera>("Camera");
        Instance.MainCamera.MakeCurrent();
        var world = GetNode<StaticBody2D>("World");
        world.InputEvent += FocusWorldCamera;
    }

    private void FocusWorldCamera(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (Input.IsActionJustPressed("ui_select"))
        {
            Instance.MainCamera.FollowTarget(this);
        }

        // Print number of cameras in the scene
        for (int i = 0; i < GetTree().Root.GetChildCount(); i++)
        {
            if (GetTree().Root.GetChild(i) is Camera camera)
            {
                GD.Print(camera.Name);
            }
        }
    }
}
