using System.Collections.Generic;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class TerrainSettings : Resource
{
    [Export(PropertyHint.Range, "-1,1,")]
    public float Min { get; set; } = 0;// min noise value
    [Export(PropertyHint.Range, "-1,1,")]
    public float Max { get; set; } = 0;// max noise value
    [Export] public Biome Biome { get; set; } = Biome.None;
    [Export] public EnvironmentLayer Layer { get; set; } = EnvironmentLayer.Surface; // tilemap layer index
    [Export] public WorldResource? Resource { get; set; } = null;

    public void GenerateAt(float value, Biome biome, Vector2I position, TileMap tilemap)
    {
        if (value < Min || value > Max)
            return;
        if (Biome != Biome.None && biome != Biome)
            return;
        Resource?.GenerateAt(position, Layer, tilemap);
    }

    public IEnumerable<string> Warnings(TileMap tilemap)
    {
        if (Min > Max)
            yield return $"{this}: Minimum noise is greater than the maximum.";
        if (Resource is null)
            yield return $"{this}: Resource is null.";
        else
            foreach (var warning in Resource.Warnings(tilemap))
                yield return $"{this}: {warning}";
    }

    public override string ToString()
    {
        return $"{GetType()}({ResourceName})";
    }
}
