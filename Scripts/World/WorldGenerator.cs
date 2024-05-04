using Godot;

namespace Scripts.World;
[Tool]
public partial class WorldGenerator : TileMap
{
    [Export]
    public FastNoiseLite Noise = new();

    [Export]
    public Vector2I MapSize = new(64, 64);

    [Export]
    public Godot.Collections.Array<TerrainConfig> TerrainConfigs = new();

    public const int TERRAIN_SET = 0;


    public override void _Ready()
    {
        Clear();
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        for (int x = 0; x < MapSize.X; x++)
        {
            for (int y = 0; y < MapSize.Y; y++)
            {
                float noiseValue = Noise.GetNoise2D(x, y);
                foreach (var terrain in TerrainConfigs)
                {
                    if (noiseValue >= terrain.Min && noiseValue <= terrain.Max)
                    {
                        var tile = new Godot.Collections.Array<Vector2I> { new(x, y) };
                        SetCellsTerrainConnect(terrain.Layer, tile, TERRAIN_SET, TerrainConfigs.IndexOf(terrain));
                    }
                }
            }
        }
    }
}
