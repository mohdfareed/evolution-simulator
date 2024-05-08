using System.Collections.Generic;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class SceneResource : WorldResource
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

    public override IEnumerable<string> Warnings(TileMap tilemap)
    {
        if (tilemap is null)
            yield break;
        if (tilemap.TileSet.GetSource(Source) is not TileSetScenesCollectionSource source)
        {
            yield return $"{this}: Source is not a tileset scene source.";
            yield break;
        }
        if (!source.HasSceneTileId(ID))
        {
            yield return $"{this}: Tile does not exist in the tileset.";
        }
    }
}
