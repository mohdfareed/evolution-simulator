using System.Collections.Generic;
using Godot;

namespace EvolutionSimulator.World;
[Tool]
[GlobalClass]
public partial class TileResource : CellResource
{
    [Export] public int Source { get; set; } = 0; // tileset source index
    [Export] public Vector2I Coordinates { get; set; } = Vector2I.Zero; // tile coordinates
    [Export] public int Alternate { get; set; } = 0; // tile alternate index

    public override void GenerateAt(Vector2I position, EnvironmentLayer layer, TileMap tilemap)
    {
        try
        {
            tilemap.SetCell((int)layer, position, Source, Coordinates, Alternate);
        }
        catch (System.Exception exception)
        {
            GD.PrintErr($"{this}: Failed to generate at {position}: ", exception);
        }
    }

    public override IEnumerable<string> Warnings(TileSet tileset)
    {
        if (tileset.GetSource(Source) is not TileSetAtlasSource source)
        {
            yield return $"{this}: Source is not a tileset atlas source.";
            yield break;
        }
        if (!source.HasTile(Coordinates))
            yield return $"{this}: Tile does not exist in the tileset.";
        if (!source.HasAlternativeTile(Coordinates, Alternate))
            yield return $"{this}: Alternate tile does not exist in the tileset.";
        foreach (var warning in base.Warnings(tileset))
            yield return warning;
    }
}
