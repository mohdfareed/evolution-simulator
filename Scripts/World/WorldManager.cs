using System.Collections.Generic;
using Godot;
using EvolutionSimulator.World;
using System.Linq;

namespace EvolutionSimulator;
[Tool]
[GlobalClass]
public partial class WorldManager : Node2D
{
    [Export] public int DayLength = 120; // length of a day in seconds
    public World.Environment Environment { get; private set; } = null!;
    private Map _map = null!;
    private DirectionalLight2D _sun = null!;

    public void GenerateWorld(Vector2I position, Vector2I size)
    {
        _map.ExpandMap(Environment.Generate(position, size).ToArray(), Environment.GetCell);
    }

    private void Initialize()
    {
        foreach (var child in GetChildren())
        {
            if (child is World.Environment environment)
                Environment = environment;
            if (child is DirectionalLight2D sun)
                _sun = sun;
            if (child is Map map)
                _map = map;
        }
    }

    public override void _Ready()
    {
        Initialize(); // initialize nodes
        // expand map when world bounds reached
        _map.WorldBoundsReached += (cell) => GenerateWorld(cell, _map.ChunkSize);
        GenerateWorld(Vector2I.Zero, Environment.WorldSize); // generate initial world
    }

    public override void _Process(double delta)
    {
        // update day/night cycle
        _sun.RotationDegrees += 360 * (float)delta / DayLength;
        _sun.RotationDegrees %= 360;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("select"))
        {
            var position = Environment.GlobalToMap(GetGlobalMousePosition());
            if (Environment.GetCell(position) is null)
                GenerateWorld(position, _map.ChunkSize);
        }
    }

    public override string[] _GetConfigurationWarnings()
    {
        Initialize(); // initialize environment
        var warnings = new List<string>();
        if (Environment is null)
            warnings.Add($"Environment node not found: {nameof(World.Environment)}");
        if (_map is null)
            warnings.Add($"Map node not found: {nameof(Map)}");
        if (_sun is null)
            warnings.Add($"Sun light not found: {nameof(DirectionalLight2D)}");
        return warnings.ToArray();
    }
}
