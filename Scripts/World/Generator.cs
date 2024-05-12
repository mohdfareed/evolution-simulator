using System;
using System.Collections.Generic;
using Godot;

namespace EvolutionSimulator.World;
[Tool]
[GlobalClass]
public partial class Generator : Node
{
    [Export] public FastNoiseLite? Noise = new();
    [Export] public CellConfig?[] Settings = Array.Empty<CellConfig>();

    public IEnumerable<CellConfig> GenerateCell(Vector2I position, TileMap tilemap)
    {
        // generate noise
        if (Noise?.GetNoise2D(position.X, position.Y) is not float noise)
            yield break;

        // generate resources
        foreach (var config in Settings)
            if (config?.Generate(noise, position, tilemap, config?.Biome) ?? false)
                if (config is not null)
                    yield return config;
    }

    public override void _Ready()
    {
        if (Noise is not null) // initialize noise
            Noise.Seed = (int)GD.Randi();
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();
        if (Noise is null)
            warnings.Add("Noise is null.");
        if (GetParent() is not TileMap tilemap)
            warnings.Add("Parent is not an Environment.");
        else
            foreach (var spec in Settings)
                if (spec is null)
                    warnings.Add("Resource is null.");
                else
                    foreach (var warning in spec.Warnings(tilemap.TileSet))
                        warnings.Add(warning);
        return warnings.ToArray();
    }
}
