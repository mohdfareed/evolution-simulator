using Godot;

namespace Scripts;
[Tool]
[GlobalClass]
public partial class WorldManager : Node2D
{
    [Export] public int DayLength = 120; // length of a day in seconds

    private DirectionalLight2D _sun = null!;
    private World.Environment _environment = null!;

    public override void _Ready()
    {
        for (int i = 0; i < GetChildCount(); i++)
        {
            Node child = GetChild(i);
            if (child is World.Environment environment)
                _environment = environment;
            if (child is DirectionalLight2D sun)
                _sun = sun;
        }
    }

    public override void _Process(double delta)
    {
        // update day/night cycle
        _sun.RotationDegrees += 360 * (float)delta / DayLength;
        _sun.RotationDegrees %= 360;
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new System.Collections.Generic.List<string>();
        if (_environment is null)
            warnings.Add("Environment node not found.");
        if (_sun is null)
            warnings.Add("Sun light not found.");
        return warnings.ToArray();
    }
}
