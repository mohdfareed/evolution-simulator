using System.Collections.Generic;
using Godot;

namespace EvolutionSimulator.World;
[Tool]
[GlobalClass]
public partial class SceneResource : CellResource
{
    [Export] public int Source { get; set; } = 0; // tileset source index
    [Export] public int ID { get; set; } = 0; // tile index

    public override void GenerateAt(Vector2I position, EnvironmentLayer layer, TileMap tilemap)
    {
        try
        {
            tilemap.SetCell((int)layer, position, Source, Vector2I.Zero, ID);
        }
        catch (System.Exception exception)
        {
            GD.PrintErr($"{this}: Failed to generate at {position}: ", exception);
        }
    }

    public override IEnumerable<string> Warnings(TileSet tileset)
    {
        if (tileset.GetSource(Source) is not TileSetScenesCollectionSource source)
            yield return $"{this}: Source is not a tileset scene source.";
        else if (!source.HasSceneTileId(ID))
            yield return $"{this}: Tile does not exist in the tileset.";
        foreach (var warning in base.Warnings(tileset))
            yield return warning;
    }
}
