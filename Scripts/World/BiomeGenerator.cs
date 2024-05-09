using System.Collections.Generic;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class BiomeGenerator : Resource
{
    [Export] public FastNoiseLite Noise = new();
    [ExportGroup("Biome Definitions")]
    [Export] public BiomeSettings Ocean = new();
    [Export] public BiomeSettings Desert = new();
    [Export] public BiomeSettings Forest = new();
    [Export] public BiomeSettings Mountain = new();

    public const EnvironmentLayer BiomeLayer = EnvironmentLayer.Ground;
    private Godot.Collections.Dictionary<Biome, BiomeSettings> _biomesSpecs = new();


    public void Initialize()
    {
        Noise.Seed = (int)GD.Randi();
        _biomesSpecs[Biome.Ocean] = Ocean;
        _biomesSpecs[Biome.Desert] = Desert;
        _biomesSpecs[Biome.Forest] = Forest;
        _biomesSpecs[Biome.Mountain] = Mountain;
    }

    public Biome GenerateAt(Vector2I position, TileMap tileMap)
    {
        var value = Noise.GetNoise2D(position.X, position.Y);
        foreach (var biome in _biomesSpecs.Keys)
            if (_biomesSpecs[biome].GenerateAt(value, position, BiomeLayer, tileMap))
                return biome;
        return Biome.None;
    }

    public IEnumerable<string> Warnings(TileMap tilemap)
    {
        if (Noise is null)
            yield return $"{this}: Noise is null.";
        foreach (var spec in new BiomeSettings?[] { Ocean, Desert, Forest, Mountain })
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
