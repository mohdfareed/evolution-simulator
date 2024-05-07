using Godot;
using Godot.Collections;

namespace Scripts.World;

public enum Biome // 4 basic biomes
{
    Ocean, // cold, wet, water
    Desert, // hot, dry, sandy
    Forest, // hot, wet, green
    Mountain, // cold, dry, rocky
}

public enum EnvironmentLayers
{
    Ground,
    Surface,
    Aerial
}

[Tool]
[GlobalClass]
public partial class Environment : TileMap
{
    [Export] public Vector2I InitialMapSize = new(64, 64);
    [Export] public BiomeGenerator BiomeGenerator = new();
    [Export] public Array<WorldGenerator?> Generators = new();

    private Dictionary<Vector2I, Biome> _environmentGrid = new();

    public bool IsOnEnvironment(Vector2 globalPosition)
    {
        // check the tile at the position
        var tile = GetCellTileData(0, GlobalToMap(globalPosition));
        return tile is not null;
    }

    public void CreateTile(Vector2 globalPosition, WorldResource resource)
    {
        CreateTile(GlobalToMap(globalPosition), resource);
    }

    public override void _Ready()
    {
        Clear();
        BiomeGenerator.Initialize();
        foreach (var generator in Generators)
            generator?.Initialize();
        GenerateWorld(Vector2I.Zero, InitialMapSize);
    }

    private void GenerateWorld(Vector2I position, Vector2I size)
    {
        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
            {
                // generate biome
                var tilePosition = new Vector2I(position.X + x, position.Y + y);
                var biome = BiomeGenerator.GenerateAt(tilePosition, this);
                if (biome is not Biome _biome)
                    continue; // skip if not initialized as biome
                _environmentGrid[new Vector2I(position.X + x, position.Y + y)] = _biome;

                // generate resources
                foreach (var generator in Generators)
                    generator?.GenerateAt(tilePosition, _biome, this);
            }
        }
    }

    private Vector2I GlobalToMap(Vector2 globalPosition)
    {
        return LocalToMap(ToLocal(globalPosition));
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new System.Collections.Generic.List<string>();
        foreach (var warning in BiomeGenerator.Warnings(this))
            warnings.Add(warning);
        foreach (var generator in Generators)
            if (generator is not null)
                foreach (var warning in generator.Warnings(this))
                    warnings.Add(warning);
        return warnings.ToArray();
    }
}
