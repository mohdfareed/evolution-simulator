using Godot;

namespace Scripts;
public partial class SimulationManager : Node2D
{
    [Export] public float PixelsPerMeter = 100f;

    public static SimulationManager Instance { get; private set; }
    public Camera MainCamera { get; private set; }
    public World.WorldManager GameWorld { get; private set; }


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
            if (child is World.WorldManager)
                Instance.GameWorld = child as World.WorldManager;
            else if (child is Camera)
                Instance.MainCamera = child as Camera;
        }

        // check if components are properly set up
        if (Instance.GameWorld == null)
        {
            GD.PrintErr("World node not found.");
            Instance.GameWorld = new World.WorldManager();
        }
        if (Instance.MainCamera == null)
        {
            GD.PrintErr("Camera node not found.");
            Instance.MainCamera = new Camera();
        }
        Instance.MainCamera.MakeCurrent();
    }
}
