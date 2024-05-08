using System.Collections.Generic;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public abstract partial class WorldResource : Resource
{
    public abstract void GenerateAt(Vector2I position, EnvironmentLayer layer, TileMap tilemap);
    public abstract IEnumerable<string> Warnings(TileMap tilemap);

    public override string ToString()
    {
        return $"{GetType()}({ResourceName})";
    }
}
