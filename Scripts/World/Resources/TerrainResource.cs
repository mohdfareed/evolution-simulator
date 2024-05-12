using System.Collections.Generic;
using Godot;

namespace EvolutionSimulator.World;
[Tool]
[GlobalClass]
public partial class TerrainResource : WorldResource
{
    [Export] public int TerrainSet { get; set; } = 0; // tileset terrain index
    [Export] public int Index { get; set; } = 0; // terrain index

    private Godot.Collections.Array<Vector2I> _tiles = new() { Vector2I.Zero };

    public override void GenerateAt(Vector2I position, EnvironmentLayer layer, TileMap tilemap)
    {
        _tiles[0] = position;
        try
        {
            tilemap.SetCellsTerrainConnect((int)layer, _tiles, TerrainSet, Index);
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
        if (TerrainSet >= tilemap.TileSet.GetTerrainSetsCount())
            yield return $"{this}: Terrain set does not exist in the tileset.";
        if (Index >= tilemap.TileSet.GetTerrainsCount(TerrainSet))
            yield return $"{this}: Terrain index does not exist in the terrain set.";
    }
}
