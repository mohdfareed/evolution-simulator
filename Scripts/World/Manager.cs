using Godot;

namespace Scripts.World;
public partial class Manager : Node2D
{
    Environment _environment = null!;

    public override void _Ready()
    {
        for (int i = 0; i < GetChildCount(); i++)
        {
            Node child = GetChild(i);
            if (child is Environment environment)
                _environment = environment;
        }
        if (_environment is null)
        {
            GD.PrintErr("Environment node not found.");
            _environment = new Environment();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        CheckCameraReset();
    }

    private void CheckCameraReset()
    {
        if (Input.IsActionJustPressed("ui_select"))
        {
            if (_environment.IsOnEnvironment(GetGlobalMousePosition()))
                SimulationManager.Instance?.MainCamera.FollowTarget(null);
        }
    }
}
