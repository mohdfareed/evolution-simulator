using System.Collections.Generic;
using Godot;

namespace Scripts;
[Tool]
[GlobalClass]
public partial class SimulationManager : Node2D
{
    [Export] public int DayLength = 120; // length of a day in seconds

    public Camera Camera { get; private set; } = null!;
    public WorldManager GameWorld { get; private set; } = null!;
    public Godot.Collections.Array<Creature> Creatures { get; } = new Godot.Collections.Array<Creature>();
    private DirectionalLight2D Sun { get; set; } = null!;

    public override void _Ready()
    {
        // initialize simulation components
        for (int i = 0; i < GetChildCount(); i++)
        {
            Node child = GetChild(i);
            if (child is WorldManager worldManager)
                GameWorld = worldManager;
            if (child is Camera camera)
                Camera = camera;
            if (child is Creature creature)
                Creatures.Add(creature);
            if (child is DirectionalLight2D sun)
                Sun = sun;
        }

        // register camera follow to creatures
        foreach (var creature in Creatures)
            creature.Selected += (creature) => Camera?.FollowTarget(creature);
        Camera?.MakeCurrent();
    }

    public override void _Process(double delta)
    {
        // update day/night cycle
        Sun.RotationDegrees += 360 * (float)delta / DayLength;
    }

    public override string[] _GetConfigurationWarnings()
    {
        // verify all components are present
        var warnings = new List<string>();
        if (GameWorld is null)
            warnings.Add("World manager not found.");
        if (Camera is null)
            warnings.Add("Camera not found.");
        if (Sun is null)
            warnings.Add("Sun light not found.");
        return warnings.ToArray(); // report any missing components
    }
}
