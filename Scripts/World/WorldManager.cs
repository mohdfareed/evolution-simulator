using System.Collections.Generic;
using Godot;

namespace Scripts.World;
public partial class WorldManager : Node2D
{
    public enum Biome
    {
        Sand,
        Grass,
        Stone,
        Blue,
        Red,
        Dark
    }

    private const int TERRAIN_SET = 0;

    private readonly Dictionary<Biome, Vector2I> _biome_food_tile = new()
    {
        { Biome.Sand, new Vector2I(12, 1) },
        { Biome.Grass, new Vector2I(12, 4) },
        { Biome.Stone, new Vector2I(12, 5) },
        { Biome.Blue, new Vector2I(12, 3) },
        { Biome.Red, new Vector2I(12, 2) }
    };

    Node _chunkLoader;
    TileMap _tileMap;


    public override void _Ready()
    {
        _chunkLoader = GetNode("ChunkLoader");
        _tileMap = GetNode<TileMap>("TileMap");

        // // ensure tilemap terrains are set
        // foreach (Biome biome in Enum.GetValues(typeof(Biome)))
        // {
        //     var terrainName = _tileMap.TileSet.GetTerrainName(TERRAIN_SET, (int)biome);
        //     if (terrainName != Enum.GetName(typeof(Biome), biome))
        //     {
        //         GD.PrintErr($"Terrain name mismatch: {biome} != {terrainName}");
        //         _tileMap.TileSet.AddTerrain(TERRAIN_SET, (int)biome);
        //     }
        // }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        CheckCameraReset();
    }

    public void LoadChunk(Node2D actor)
    {
        _chunkLoader.Set("actor", actor);
        _chunkLoader.Call("_process", 0f);
    }

    private void CheckCameraReset()
    {
        if (Input.IsActionJustPressed("ui_select"))
        {
            // get tilemap click position
            var mousePos = _tileMap.ToLocal(GetGlobalMousePosition());
            var tilePos = _tileMap.LocalToMap(mousePos);

            // get the tile at the clicked position
            var tile = _tileMap.GetCellTileData(0, tilePos);
            if (tile is not null) // clear camera target if world tile is clicked
                SimulationManager.Instance.MainCamera.FollowTarget(null);
        }
    }
}
