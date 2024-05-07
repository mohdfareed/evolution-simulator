using System.Collections.Generic;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class GenerationSpec : Resource
{
    [Export(PropertyHint.Range, "-1,1,")]
    public float Min { get; set; } = 0;// min noise value
    [Export(PropertyHint.Range, "-1,1,")]
    public float Max { get; set; } = 0;// max noise value

    [Export] public Biome Biome { get; set; } // biome on which to generate
    [Export] public WorldResource Resource { get; set; } = new TileResource(); // resource to generate

    public void Initialize()
    {
        Resource ??= new TileResource();
    }

    public bool GenerateAt(float value, Vector2I position, TileMap tilemap)
    {
        if (value >= Min && value <= Max)
            return Resource.GenerateAt(position, tilemap);
        return false;
    }

    public IEnumerable<string> Warnings(TileMap tilemap)
    {
        if (Min > Max)
            yield return $"{this}: min is greater than max.";
        if (Resource is null)
            yield return $"{this}: resource is null.";
        else
            foreach (var warning in Resource.Warnings(tilemap))
                yield return warning;
    }
}
