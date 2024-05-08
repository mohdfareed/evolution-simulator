using System;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class Environment : TileMap
{
    [Export] public Vector2I InitialMapSize = new(64, 64);
    [Export] public BiomeGenerator BiomeGenerator = new();
    [Export] public WorldGenerator?[] Generators = Array.Empty<WorldGenerator>();

    private Godot.Collections.Dictionary<Vector2I, Biome> _biomeMap = new();

    public override void _Ready()
    {
        // initialize environment
        Initialize();
        BiomeGenerator.Initialize();
        foreach (var generator in Generators)
            generator?.Initialize(BiomeGenerator.Noise);

        // generate world
        Clear(); GenerateInitialWorld(Vector2I.Zero, InitialMapSize);
    }

    private void GenerateInitialWorld(Vector2I position, Vector2I size)
    {
        for (int x = -size.X / 2; x < size.X / 2; x++)
        {
            for (int y = -size.Y / 2; y < size.Y / 2; y++)
            {
                // generate biome
                var tilePosition = new Vector2I(position.X + x, position.Y + y);
                _biomeMap[tilePosition] = BiomeGenerator.GenerateAt(tilePosition, this);

                // generate world based on biome
                foreach (var generator in Generators)
                    generator?.GenerateAt(tilePosition, _biomeMap[tilePosition], this);
            }
        }
    }

    private Vector2I GlobalToMap(Vector2 globalPosition)
    {
        return LocalToMap(ToLocal(globalPosition));
    }

    private void Initialize()
    {
        // ensure enough layers exist
        for (int i = GetLayersCount(); i < Enum.GetNames(typeof(EnvironmentLayer)).Length; i++)
            AddLayer(i);

        // configure layers
        for (int i = 0; i < Enum.GetNames(typeof(EnvironmentLayer)).Length; i++)
        {
            SetLayerZIndex(i, i);
            SetLayerName(i, Enum.GetName(typeof(EnvironmentLayer), i));
        }
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new System.Collections.Generic.List<string>();
        foreach (var warning in BiomeGenerator.Warnings(this))
            warnings.Add(warning);
        foreach (var generator in Generators)
            if (generator is null)
                warnings.Add("Generator is null.");
            else
                foreach (var warning in generator.Warnings(this))
                    warnings.Add(warning);
        return warnings.ToArray();
    }
}

public enum Biome // 4 basic biomes
{
    Ocean, // cold, wet, water
    Desert, // hot, dry, sandy
    Forest, // hot, wet, green
    Mountain, // cold, dry, rocky
    None // biome agnostic
}

public enum EnvironmentLayer
{
    Ground,
    Surface,
    Air
}
