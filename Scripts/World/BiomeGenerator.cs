using System.Collections.Generic;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class BiomeGenerator : Resource
{
    [Export] public FastNoiseLite Noise = new();
    [ExportGroup("Biome Definitions")]
    [Export] public GenerationSpec Ocean = new() { Biome = Biome.Ocean };
    [Export] public GenerationSpec Desert = new() { Biome = Biome.Desert };
    [Export] public GenerationSpec Forest = new() { Biome = Biome.Forest };
    [Export] public GenerationSpec Mountain = new() { Biome = Biome.Mountain };

    private Godot.Collections.Dictionary<Biome, GenerationSpec> _biomesSpecs = new();


    public void Initialize()
    {
        Noise.Seed = (int)GD.Randi();
        _biomesSpecs[Ocean.Biome] = Ocean;
        _biomesSpecs[Desert.Biome] = Desert;
        _biomesSpecs[Forest.Biome] = Forest;
        _biomesSpecs[Mountain.Biome] = Mountain;

        foreach (var biome in _biomesSpecs.Keys)
        {
            _biomesSpecs[biome].Biome = biome;
            _biomesSpecs[biome].Resource.Layer = EnvironmentLayers.Ground;
        }
    }

    public Biome? GenerateAt(Vector2I position, TileMap tileMap)
    {
        var value = Noise.GetNoise2D(position.X, position.Y);
        Biome? biome = null;
        foreach (var spec in _biomesSpecs.Values)
            biome = spec.GenerateAt(value, position, tileMap) ? spec.Biome : biome;
        return biome;
    }

    public IEnumerable<string> Warnings(TileMap tilemap)
    {
        foreach (var spec in new GenerationSpec[] { Ocean, Desert, Forest, Mountain })
            if (spec is null)
                yield return "Generation spec is null.";
            else
                foreach (var warning in spec.Warnings(tilemap))
                    yield return warning;
    }
}
