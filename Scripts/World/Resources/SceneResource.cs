using System.Collections.Generic;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class SceneResource : WorldResource
{
    [Export] public int Source { get; set; } = 0; // tileset source index
    [Export] public int ID { get; set; } = 0; // tile index

    public override bool GenerateAt(Vector2I position, TileMap tilemap)
    {
        try
        {
            tilemap.SetCell((int)Layer, position, Source, Vector2I.Zero, ID);
            return true;
        }
        catch (System.Exception exception)
        {
            GD.PrintErr($"{this.GetType()}: failed to generate at {position}.", exception);
            return false;
        }
    }

    public override IEnumerable<string> Warnings(TileMap tilemap)
    {
        foreach (var warning in base.Warnings(tilemap))
            yield return warning;
        if (tilemap is null)
            yield break;
        if (tilemap.TileSet.GetSource(Source) is not TileSetScenesCollectionSource source)
        {
            yield return $"{this.GetType()}: source is not a tileset scene source.";
            yield break;
        }
        if (!source.HasSceneTileId(ID))
        {
            yield return $"{this.GetType()}: tile does not exist in the tileset.";
        }
    }
}
