using Godot;

namespace Scripts.World;
[Tool]
public partial class Environment : TileMap
{
    [Export]
    public Godot.Collections.Array<Generator> Generators = new();
    [Export]
    public Vector2I MapSize = new(64, 64);

    public const int TERRAIN_SET = 0;


    public override void _Ready()
    {
        Clear();
        GenerateWorld();
    }

    public bool IsOnEnvironment(Vector2 globalPosition)
    {
        // get tile position
        var localPos = ToLocal(globalPosition);
        var tilePos = LocalToMap(localPos);

        // check the tile at the position
        var tile = GetCellTileData(0, tilePos);
        return tile is not null;
    }

    private void GenerateWorld()
    {
        for (int x = 0; x < MapSize.X; x++)
        {
            for (int y = 0; y < MapSize.Y; y++)
            {
                // generate resources for the tile
                Godot.Collections.Array<GeneratedResource?> resources = new();
                foreach (var Generator in Generators)
                    if (Generator is not null)
                        resources.AddRange(Generator.GenerateAt(x, y));

                // create tiles for the resources
                foreach (var resource in resources)
                {
                    if (resource is null)
                        continue; // skip empty tiles
                    CreateTile(new Vector2I(x, y), resource);
                }
            }
        }
    }

    private void CreateTile(Vector2I MapCoord, GeneratedResource resource)
    {
        if (resource is TerrainResource terrain)
        {
            var tiles = new Godot.Collections.Array<Vector2I> { MapCoord };
            SetCellsTerrainConnect(terrain.Layer, tiles, terrain.TerrainSet, terrain.Index);
        }
        if (resource is SceneResource scene)
            SetCell(scene.Layer, MapCoord, scene.Source, Vector2I.Zero, scene.Index);
        if (resource is TileResource tile)
            SetCell(tile.Layer, MapCoord, tile.Source, tile.Coordinates, tile.Alternate);
    }
}
