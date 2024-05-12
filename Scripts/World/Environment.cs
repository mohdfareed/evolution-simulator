using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace EvolutionSimulator.World;
[Tool]
[GlobalClass]
public partial class Environment : TileMap
{
    [Export] public Vector2I WorldSize = new(32, 32); // initial world size

    private readonly Dictionary<Vector2I, Cell> _biomeMap = new();
    private Generator[] _generators = Array.Empty<Generator>();

    public Cell? this[Vector2I position] => GetCell(position);
    public Cell? this[int x, int y] => GetCell(new Vector2I(x, y));

    public IEnumerable<Vector2I> Generate(Vector2I position, Vector2I size, bool overwrite = false)
    {
        for (int x = position.X - size.X / 2; x < position.X + size.X / 2; x++)
            for (int y = position.Y - size.Y / 2; y < position.Y + size.Y / 2; y++)
            {
                GenerateCell(new Vector2I(x, y), overwrite);
                yield return new Vector2I(x, y);
            }
    }

    public void GenerateCell(Vector2I position, bool overwrite = false)
    {
        if (!overwrite && _biomeMap.ContainsKey(position))
            return;

        CellConfig? cell = null;
        foreach (var generator in _generators)
            cell = generator.GenerateCell(position, this).Aggregate(cell, (mergedCell, newCell) =>
            {
                mergedCell?.MergeCellConfig(newCell);
                return mergedCell ?? newCell;
            });
        if (cell is not null)
            _biomeMap[position] = cell.Create(position);
    }

    public IEnumerable<Cell> GetCells(Vector2I position, Vector2I size)
    {
        for (int x = position.X - size.X / 2; x < position.X + size.X / 2; x++)
            for (int y = position.Y - size.Y / 2; y < position.Y + size.Y / 2; y++)
                if (GetCell(new Vector2I(x, y)) is Cell cell)
                    yield return cell;
    }

    public Cell? GetCell(Vector2I position)
    {
        var cell = GetCellTileData((int)EnvironmentLayer.Ground, position);
        return _biomeMap.GetValueOrDefault(position);
    }

    public bool IsSurfaceEmpty(Vector2I position)
    {
        return GetCellTileData((int)EnvironmentLayer.Surface, position) is null;
    }

    public Vector2I GlobalToMap(Vector2 position)
    {
        return LocalToMap(ToLocal(position));
    }

    public Vector2 MapToGlobal(Vector2I mapPosition, bool center = true)
    {
        if (center)
            return ToGlobal(MapToLocal(mapPosition));
        return ToGlobal(MapToLocal(mapPosition + (TileSet.TileSize / 2)));
    }

    private void Initialize()
    {
        // ensure enough layers exist
        for (int i = GetLayersCount(); i < Enum.GetNames(typeof(EnvironmentLayer)).Length; i++)
            AddLayer(i);

        // configure layers
        for (int i = 0; i < Enum.GetNames(typeof(EnvironmentLayer)).Length; i++)
        {
            SetLayerZIndex(i, i);
            SetLayerName(i, Enum.GetName(typeof(EnvironmentLayer), i));
        }

        // initialize generators
        _generators = GetChildren().Where(child =>
            child is Generator generator).Cast<Generator>().ToArray();
    }

    public override void _Ready()
    {
        Initialize(); // initialize environment
        Clear(); // start clean world
    }

    public override string[] _GetConfigurationWarnings()
    {
        Initialize(); // initialize environment
        var warnings = new List<string>();
        if (WorldSize.X <= 0 || WorldSize.Y <= 0)
            warnings.Add("World size is invalid.");
        return warnings.ToArray();
    }
}
