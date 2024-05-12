using System;
using System.Collections.Generic;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class TerrainGenerator : Resource
{
    [Export] public FastNoiseLite? Noise = new(); // use biome noise if null
    [Export] public TerrainSettings?[] Specs = Array.Empty<TerrainSettings>();

    public void Initialize()
    {
        if (Noise is not null)
            Noise.Seed = (int)GD.Randi();
    }

    public void GenerateAt(Vector2I position, Biome biome, TileMap tileMap)
    {
        // get normalized noise value
        var value = Noise?.GetNoise2D(position.X, position.Y);
        if (value is not float noise)
            return;

        foreach (var spec in Specs)
            spec?.GenerateAt(noise, biome, position, tileMap);
    }

    public IEnumerable<string> Warnings(TileMap tilemap)
    {
        if (Noise is null)
            yield return $"{this}: No noise provided.";
        foreach (var spec in Specs)
            if (spec is null)
                yield return $"{this}: Generation spec is null.";
            else
                foreach (var warning in spec.Warnings(tilemap))
                    yield return $"{this}: {warning}";
    }

    public override string ToString()
    {
        return $"{GetType()}({ResourceName})";
    }
}
