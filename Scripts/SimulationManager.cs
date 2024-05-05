using Godot;

namespace Scripts;
public partial class SimulationManager : Node2D
{
    public static SimulationManager? Instance { get; private set; } = null;
    public Camera MainCamera { get; private set; } = null!;
    public World.Manager GameWorld { get; private set; } = null!;


    public override void _EnterTree()
    {
        if (Instance != null)
        {
            GD.PrintErr("SimulationManager is a singleton and cannot have more than one instance.");
            QueueFree();  // destroy the redundant instance
            return;
        }
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public override void _Ready()
    {
        // load game world and main camera
        for (int i = 0; i < GetChildCount(); i++)
        {
            Node child = GetChild(i);
            if (child is World.Manager worldManager)
                Instance!.GameWorld = worldManager;
            else if (child is Camera camera)
                Instance!.MainCamera = camera;
        }

        // check if components are properly set up
        if (Instance!.GameWorld == null)
        {
            GD.PrintErr("World node not found.");
            Instance.GameWorld = new World.Manager();
        }
        if (Instance.MainCamera == null)
        {
            GD.PrintErr("Camera node not found.");
            Instance.MainCamera = new Camera();
        }
        Instance.MainCamera.MakeCurrent();
    }
}
