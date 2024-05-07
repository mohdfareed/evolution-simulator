using System.Collections.Generic;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class TileResource : WorldResource
{
    [Export] public int Source { get; set; } = 0; // tileset source index
    [Export] public Vector2I Coordinates { get; set; } = Vector2I.Zero; // tile coordinates
    [Export] public int Alternate { get; set; } = 0; // tile alternate index

    public override bool GenerateAt(Vector2I position, TileMap tilemap)
    {
        try
        {
            tilemap.SetCell((int)Layer, position, Source, Coordinates, Alternate);
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
        if (tilemap is null)
            yield break;
        if (tilemap.TileSet.GetSource(Source) is not TileSetAtlasSource source)
        {
            yield return $"{this.GetType()}: source is not a tileset atlas source.";
            yield break;
        }
        if (!source.HasTile(Coordinates) || !source.HasAlternativeTile(Coordinates, Alternate))
        {
            yield return $"{this.GetType()}: tile does not exist in the tileset.";
        }
    }
}
