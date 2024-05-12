using System;
using System.Collections.Generic;
using Godot;

namespace EvolutionSimulator.World;
[Tool]
[GlobalClass]
public partial class CellConfig : GodotObject
{
    // generation properties
    [Export(PropertyHint.Range, "-1,1,")]
    public float Min { get; set; } = 0; // min noise value
    [Export(PropertyHint.Range, "-1,1,")]
    public float Max { get; set; } = 0; // max noise value
    [Export(PropertyHint.Range, "0,1,")]
    public float Probability { get; set; } = 1; // probability of generating
    [Export] public bool RestrictBiome { get; set; } = false; // restrict to biome
    [Export] CellResource? Resource { get; set; } = null!; // resource

    // cell properties
    [Export] public EnvironmentLayer Layer { get; set; } = EnvironmentLayer.Ground;
    [Export] public Biome Biome { get; set; } = Biome.None;
    [Export] public float StartingResources { get; set; } = -1;

    public bool Generate(float noise, Vector2I position, TileMap tilemap, Biome? currentBiome)
    {
        if (RestrictBiome && currentBiome != Biome.None && Biome != currentBiome)
        {
            GD.Print($"{this}: Biome mismatch: {Biome} != {currentBiome}");
            return false;
        }
        if (Min < noise && noise < Max)
            if (GD.Randf() <= Probability)
            {
                Resource?.GenerateAt(position, Layer, tilemap);
                return true;
            }
        return false;
    }

    public void MergeCellConfig(CellConfig other)
    {
        if (other.Biome != Biome.None)
            Biome = other.Biome;
        if (other.StartingResources >= 0)
            StartingResources = other.StartingResources;
    }

    public Cell Create(Vector2I position)
    {
        return new Cell()
        {
            Coord = position,
            Biome = Biome,
            Resources = StartingResources,
            Layer = Layer
        };
    }

    public virtual IEnumerable<string> Warnings(TileSet tileset)
    {
        if (Min > Max)
            yield return $"{this}: Minimum noise is greater than the maximum.";
        if (Resource is null)
            yield return $"{this}: Resource is null.";
        foreach (var warning in Resource?.Warnings(tileset) ?? Array.Empty<string>())
            yield return $"{this}: {warning}";
        yield break;
    }

    public override string ToString()
    {
        return $"{GetType()}()";
    }
}
