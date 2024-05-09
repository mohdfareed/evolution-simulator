using System.Collections.Generic;
using Godot;

namespace Scripts;
[GlobalClass]
public partial class SimulationManager : Node
{
    public Camera Camera { get; private set; } = null!;
    public WorldManager GameWorld { get; private set; } = null!;
    public Godot.Collections.Array<Creature> Creatures { get; } = new Godot.Collections.Array<Creature>();

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
        }

        // register camera follow to creatures
        foreach (var creature in Creatures)
            creature.Selected += (creature) => Camera?.FollowTarget(creature);
        Camera?.MakeCurrent();
    }

    public override string[] _GetConfigurationWarnings()
    {
        // verify all components are present
        var warnings = new List<string>();
        if (GameWorld is null)
            warnings.Add("World manager not found.");
        if (Camera is null)
            warnings.Add("Camera not found.");
        return warnings.ToArray(); // report any missing components
    }
}
