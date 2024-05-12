using System.Collections.Generic;
using Godot;

namespace EvolutionSimulator.World;
[Tool]
[GlobalClass]
public abstract partial class CellResource : Godot.Resource
{
    public abstract void GenerateAt(Vector2I position, EnvironmentLayer layer, TileMap tilemap);

    public virtual IEnumerable<string> Warnings(TileSet tileset)
    {
        if (tileset is null)
            yield return $"{this}: Tileset is null.";
    }

    public override string ToString()
    {
        return $"{GetType()}({ResourceName})";
    }
}
