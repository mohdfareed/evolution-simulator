using System.Collections.Generic;
using Godot;

namespace EvolutionSimulator.World;
[Tool]
[GlobalClass]
public partial class TerrainResource : CellResource
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

    public override IEnumerable<string> Warnings(TileSet tileset)
    {
        if (TerrainSet >= tileset.GetTerrainSetsCount())
            yield return $"{this}: Terrain set does not exist in the tileset.";
        if (Index >= tileset.GetTerrainsCount(TerrainSet))
            yield return $"{this}: Terrain index does not exist in the terrain set.";
        foreach (var warning in base.Warnings(tileset))
            yield return warning;
    }
}
