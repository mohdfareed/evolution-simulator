using System;
using System.Collections.Generic;
using Godot;

namespace EvolutionSimulator.World;
[Tool]
[GlobalClass]
public partial class Environment : TileMap
{
    [Export] public BiomeGenerator? BiomeGenerator = new();
    [Export] public TerrainGenerator?[] Generators = Array.Empty<TerrainGenerator>();

    public Vector2I CellSize => TileSet.TileSize;

    private readonly Dictionary<Vector2I, Biome> _biomeMap = new();

    public Biome GenerateCell(Vector2I position, bool overwrite = false)
    {
        if (!overwrite && _biomeMap.TryGetValue(position, out var biome) && biome != Biome.None)
            return Biome.None;  // return if not overwriting and a biome already exists
        // generate biome
        _biomeMap[position] = BiomeGenerator?.GenerateAt(position, this) ?? Biome.None;
        // generate world based on biome
        foreach (var generator in Generators)
            generator?.GenerateAt(position, _biomeMap[position], this);
        return _biomeMap[position];
    }

    public Vector2I GlobalToMap(Vector2 position)
    {
        return LocalToMap(ToLocal(position));
    }

    public Vector2 MapToGlobal(Vector2I mapPosition)
    {
        return ToGlobal(MapToLocal(mapPosition));
    }

    public Vector2 MapToGlobalCorner(Vector2I mapPosition)
    {
        return ToGlobal(MapToLocal(mapPosition + (CellSize / 2)));
    }

    public Biome GetBiome(Vector2I position)
    {
        return _biomeMap.TryGetValue(position, out var biome) ? biome : Biome.None;
    }

    public bool IsSurfaceEmpty(Vector2I position)
    {
        return GetCellTileData((int)EnvironmentLayer.Surface, position) is null;
    }

    public override void _Ready()
    {
        TileSet ??= new TileSet();
        BiomeGenerator ??= new BiomeGenerator();

        // ensure enough layers exist
        for (int i = GetLayersCount(); i < Enum.GetNames(typeof(EnvironmentLayer)).Length; i++)
            AddLayer(i);

        // configure layers
        for (int i = 0; i < Enum.GetNames(typeof(EnvironmentLayer)).Length; i++)
        {
            SetLayerZIndex(i, i);
            SetLayerName(i, Enum.GetName(typeof(EnvironmentLayer), i));
        }

        // initialize generators
        BiomeGenerator?.Initialize();
        foreach (var generator in Generators)
            generator?.Initialize();
        Clear(); // start clean world
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();
        if (TileSet is null)
            warnings.Add("Tileset is null.");
        if (BiomeGenerator is null)
            warnings.Add("Biome generator is null.");
        else
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

public enum EnvironmentLayer
{
    Ground,
    Surface,
    Air
}
