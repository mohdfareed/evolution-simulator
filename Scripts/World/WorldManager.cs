using System.Collections.Generic;
using Godot;
using Scripts.World;

namespace Scripts;
[Tool]
[GlobalClass]
public partial class WorldManager : Node2D
{
    [Export] public int DayLength = 120; // length of a day in seconds
    [Export] public Vector2I WorldSize = new(32, 32); // initial world size
    [Export] public Vector2I ChunkSize = new(8, 8); // size of a chunk in tiles

    private World.Environment _environment = null!;
    private Map _map = null!;
    private DirectionalLight2D _sun = null!;

    private void GenerateWorld(Vector2I position, Vector2I size)
    {
        // generate biome
        var biomeMap = new Dictionary<Vector2I, Biome>();
        for (int x = position.X - size.X / 2; x < position.X + size.X / 2; x++)
            for (int y = position.Y - size.Y / 2; y < position.Y + size.Y / 2; y++)
            {
                var mapPosition = new Vector2I(x, y);
                var biome = _environment.GenerateCell(mapPosition);
                biomeMap[mapPosition] = biome;
            }

        // generate map
        _map.ExpandMap(biomeMap, _environment.MapToGlobalCorner);
    }

    public override void _Ready()
    {
        for (int i = 0; i < GetChildCount(); i++)
        {
            Node child = GetChild(i);
            if (child is World.Environment environment)
                _environment = environment;
            if (child is DirectionalLight2D sun)
                _sun = sun;
            if (child is Map map)
                _map = map;
        }

        // connect to map expansion
        _map.WorldBoundsReached += (body) =>
            GenerateWorld(_environment.GlobalToMap(body.GlobalPosition), ChunkSize);
        // generate initial world
        GenerateWorld(Vector2I.Zero, WorldSize);
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
            var position = _environment.GlobalToMap(GetGlobalMousePosition());
            if (_environment.GetBiome(position) == Biome.None)
                GenerateWorld(position, ChunkSize);
        }
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();
        if (_environment is null)
            warnings.Add($"Environment node not found: {nameof(World.Environment)}");
        if (_map is null)
            warnings.Add($"Map node not found: {nameof(Map)}");
        if (_sun is null)
            warnings.Add($"Sun light not found: {nameof(DirectionalLight2D)}");
        return warnings.ToArray();
    }
}
