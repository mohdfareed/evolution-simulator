using System.Collections.Generic;
using Godot;

namespace EvolutionSimulator.World;
[Tool]
[GlobalClass]
public partial class BiomeSettings : Godot.Resource
{
    [Export(PropertyHint.Range, "-1,1,")]
    public float Limit { get; set; } = 0;// noise limit to generate
    [Export] public EnvironmentLayer Layer { get; set; } = EnvironmentLayer.Ground; // tilemap layer index
    [Export] public WorldResource? Resource { get; set; }


    public bool GenerateAt(float value, Vector2I position, EnvironmentLayer layer, TileMap tilemap)
    {
        if (value > Limit)
            return false;
        Resource?.GenerateAt(position, layer, tilemap);
        return Resource is not null;
    }

    public IEnumerable<string> Warnings(TileMap tilemap)
    {
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
