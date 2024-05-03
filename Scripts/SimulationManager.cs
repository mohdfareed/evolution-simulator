using System;
using Godot;

namespace Scripts;
public partial class SimulationManager : Node2D
{
    [Export] public float PixelsPerMeter = 100f;

    public static SimulationManager Instance { get; private set; }
    public Camera MainCamera { get; private set; }
    public World World { get; private set; }


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
        // load world
        Instance.World = GetNode<World>("World");
        if (Instance.World == null)
            throw new NullReferenceException("World node not found.");

        // load main camera
        Instance.MainCamera = GetNode<Camera>("Camera");
        if (Instance.MainCamera == null)
            throw new NullReferenceException("Camera node not found.");
        Instance.MainCamera.MakeCurrent();
    }
}
