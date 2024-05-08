using System;
using System.Collections.Generic;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class WorldGenerator : Resource
{
    [Export] public FastNoiseLite? Noise = new(); // use biome noise if null
    [Export] public GenerationSettings?[] Specs = Array.Empty<GenerationSettings>();

    private FastNoiseLite? biomeNoise;

    public void Initialize(FastNoiseLite biomeNoise)
    {
        if (Noise is not null)
            Noise.Seed = (int)GD.Randi();
        else
            this.biomeNoise = biomeNoise;
    }

    public void GenerateAt(Vector2I position, Biome biome, TileMap tileMap)
    {
        float noise;
        if (Noise is not null)
            noise = Noise.GetNoise2D(position.X, position.Y);
        else if (biomeNoise is not null)
            noise = biomeNoise.GetNoise2D(position.X, position.Y);
        else
        {
            GD.PrintErr($"{this}: No noise provided.");
            return;
        }

        foreach (var spec in Specs)
            spec?.GenerateAt(noise, biome, position, tileMap);
    }

    public IEnumerable<string> Warnings(TileMap tilemap)
    {
        if (Noise is null && biomeNoise is null)
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
