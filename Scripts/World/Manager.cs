using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
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
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new System.Collections.Generic.List<string>();
        if (_environment is null)
            warnings.Add("Environment node not found.");
        return warnings.ToArray();
    }
}
