using Godot;

namespace Scripts;
[Tool]
[GlobalClass]
public partial class SimulationManager : Node2D
{
    public Camera Camera { get; private set; } = null!;
    public World.Manager GameWorld { get; private set; } = null!;
    public Godot.Collections.Array<Creature> Creatures { get; } = new Godot.Collections.Array<Creature>();

    public override void _Ready()
    {
        // initialize simulation components
        Initialize();
        if (Engine.IsEditorHint())
            return; // skip initialization in editor

        // register camera follow to creatures
        foreach (var creature in Creatures)
            creature.Selected += (creature) => Camera?.FollowTarget(creature);
        Camera?.MakeCurrent();
    }

    private void Initialize()
    {
        // initialize simulation components
        for (int i = 0; i < GetChildCount(); i++)
        {
            Node child = GetChild(i);
            if (child is World.Manager worldManager)
                GameWorld = worldManager;
            if (child is Camera camera)
                Camera = camera;
            if (child is Creature creature)
                Creatures.Add(creature);
        }
    }

    public override string[] _GetConfigurationWarnings()
    {
        // initialize simulation and report any warnings
        var warnings = new System.Collections.Generic.List<string>();
        // verify all components are present
        if (GameWorld is null)
            warnings.Add("World manager not found.");
        if (Camera is null)
            warnings.Add("Camera not found.");
        // report any missing components
        return warnings.ToArray();
    }
}
