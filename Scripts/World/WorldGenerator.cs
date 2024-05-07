using System.Collections.Generic;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class WorldGenerator : Resource
{
    [Export] public FastNoiseLite Noise = new();
    [Export] public Godot.Collections.Array<GenerationSpec?> Specs = new();

    public void Initialize()
    {
        Noise.Seed = (int)GD.Randi();
    }

    public void GenerateAt(Vector2I position, Biome biome, TileMap tileMap)
    {
        var value = Noise.GetNoise2D(position.X, position.Y);
        foreach (var spec in Specs)
        {
            if (spec?.Biome == biome)
            {
                spec.GenerateAt(value, position, tileMap);
            }
        }
    }

    public IEnumerable<string> Warnings(TileMap tilemap)
    {
        foreach (var spec in Specs)
            if (spec is null)
                yield return "Generation spec is null.";
            else
                foreach (var warning in spec.Warnings(tilemap))
                    yield return warning;
    }
}
