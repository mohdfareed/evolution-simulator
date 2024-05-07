using System.Collections.Generic;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class TerrainResource : WorldResource
{
    [Export] public int TerrainSet { get; set; } = 0; // tileset terrain index
    [Export] public int Index { get; set; } = 0; // terrain index

    // private Godot.Collections.Array<Vector2I> _tiles = new() { Vector2I.Zero };

    public override bool GenerateAt(Vector2I position, TileMap tilemap)
    {
        var _tiles = new Godot.Collections.Array<Vector2I> { position };
        try
        {
            tilemap.SetCellsTerrainConnect((int)Layer, _tiles, TerrainSet, Index);
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

        // check if terrain exists in the tileset
        var validSet = TerrainSet < tilemap.TileSet.GetTerrainSetsCount();
        if (!validSet)
        {
            yield return $"{this.GetType()}: terrain set does not exist in the tileset.";
        }
        var validIndex = Index < tilemap.TileSet.GetTerrainsCount(TerrainSet);
        if (!validIndex)
        {
            yield return $"{this.GetType()}: terrain index does not exist in the terrain set.";
        }
    }
}
