using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class BiomeGenerator : Resource
{
    [Export] public FastNoiseLite? Noise = new();
    [ExportGroup("Biome Definitions")]
    [Export] public BiomeSettings?[] Ocean = Array.Empty<BiomeSettings>();
    [Export] public BiomeSettings?[] Desert = Array.Empty<BiomeSettings>();
    [Export] public BiomeSettings?[] Forest = Array.Empty<BiomeSettings>();
    [Export] public BiomeSettings?[] Mountain = Array.Empty<BiomeSettings>();
    // [Export] public BiomeSettings? Ocean = null!;
    // [Export] public BiomeSettings? Desert = null!;
    // [Export] public BiomeSettings? Forest = null!;
    // [Export] public BiomeSettings? Mountain = null!;

    public const EnvironmentLayer BiomeLayer = EnvironmentLayer.Ground;
    private Dictionary<Biome, BiomeSettings?[]> _biomesSpecs = new();
    // private Godot.Collections.Dictionary<Biome, BiomeSettings?> _biomesSpecs = new();


    public void Initialize()
    {
        _biomesSpecs[Biome.Ocean] = Ocean;
        _biomesSpecs[Biome.Desert] = Desert;
        _biomesSpecs[Biome.Forest] = Forest;
        _biomesSpecs[Biome.Mountain] = Mountain;
        if (Noise is not null)
            Noise.Seed = (int)GD.Randi();
    }

    public Biome GenerateAt(Vector2I position, TileMap tileMap)
    {
        // get normalized noise value
        var value = Noise?.GetNoise2D(position.X, position.Y);
        if (value is not float noise)
            return Biome.None;

        // generate biome
        foreach (var biome in _biomesSpecs.Keys)
            foreach (var biomeSpec in _biomesSpecs[biome])
                if (biomeSpec?.GenerateAt(noise, position, BiomeLayer, tileMap) ?? false)
                    // if (_biomesSpecs[biome]?.GenerateAt(noise, position, BiomeLayer, tileMap) ?? false)
                    return biome;
        return Biome.None;
    }

    public IEnumerable<string> Warnings(TileMap tilemap)
    {
        if (Noise is null)
            yield return $"{this}: Noise is null.";
        foreach (var spec in Ocean.Concat(Desert).Concat(Forest).Concat(Mountain))
            // foreach (var spec in _biomesSpecs.Values)
            if (spec is null)
                yield return $"{this}: Generation settings is null.";
            else
                foreach (var warning in spec.Warnings(tilemap))
                    yield return $"{this}: {warning}";
    }

    public override string ToString()
    {
        return $"{GetType()}({ResourceName})";
    }
}
