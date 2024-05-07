using System.Collections.Generic;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public abstract partial class WorldResource : Resource
{
    [Export] public EnvironmentLayers Layer { get; set; } = EnvironmentLayers.Surface; // tilemap layer index

    public abstract bool GenerateAt(Vector2I position, TileMap tilemap);

    public virtual IEnumerable<string> Warnings(TileMap tilemap)
    {
        if (Layer < 0)
            yield return $"{this}: Tilemap layer is negative.";

    }
}
