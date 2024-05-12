using System;
using System.Collections.Generic;
using Godot;

namespace EvolutionSimulator.World;
[Tool]
[GlobalClass]
public partial class Map : Node2D
{
    [Export] public int WorldBoundaryMargin = 4; // in tiles
    [Export] public float ExpansionCoolDown = 2.5f; // in seconds
    [Export] public Vector2I ChunkSize = new(8, 8); // size of a chunk in tiles
    [Export] public CellResource? BordersResource = null!;
    [Signal] public delegate void WorldBoundsReachedEventHandler(Vector2I cell);
    private bool _isOnCooldown = false;

    public void ExpandMap(IEnumerable<Vector2I> generatedCells, Func<Vector2I, Cell?> _environment)
    {
        // find world borders
        foreach (var cellCoord in generatedCells)
        {
            if (_environment(cellCoord) is not Cell cell)
                continue; // skip if cell is not found

            // unsubscribe from previous events
            cell.Entered -= OnWorldBoundsReached;
            currentBounds.Remove(cell.Coord);

            // subscribe to world bounds
            if (IsCellWorldBound(cell, _environment))
            {
                cell.Entered += OnWorldBoundsReached;
                currentBounds.Add(cell.Coord);
            }
        }
    }

    private bool IsCellWorldBound(Cell cell, Func<Vector2I, Cell?> _environment)
    {
        for (int i = 0; i < WorldBoundaryMargin; i++) // check each neighbor up to the margin
            foreach (var neighborCoord in cell.Neighbors(i + 1)) // check all directions for world bounds
                if (_environment(neighborCoord) is null)
                    return true; // world bounds neighbor nothing
        return false;
    }

    private async void OnWorldBoundsReached(Cell cell, Node body)
    {
        if (_isOnCooldown)
            return;

        // notify world bounds reached
        EmitSignal(SignalName.WorldBoundsReached, cell.Coord);

        // cooldown expansion
        var timer = GetTree().CreateTimer(ExpansionCoolDown);
        _isOnCooldown = true;
        await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        _isOnCooldown = false;
    }

    private List<Vector2I> currentBounds = new();
    public override void _Process(double _) => QueueRedraw();
    public override void _Draw()
    {
        // clear previous bounds
        foreach (var cell in currentBounds)
            DrawRect(new Rect2(cell * 128, Vector2.One * 128), new Color(0, 0, 0, 0));
        // draw squares 128x128 for each point
        foreach (var cell in currentBounds)
            DrawRect(new Rect2(cell * 128, Vector2.One * 128), new Color(1, 0, 0.25f, 0.5f));
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();
        if (ExpansionCoolDown <= 0)
            warnings.Add("Expansion cooldown is not valid.");
        return warnings.ToArray();
    }
}

public class BorderEffect : ICellEffect
{
    public CellResource? Resource { get; set; }
    public Environment? Environment { get; set; } = null;
    private Dictionary<Cell, float> _resources = new();

    public void Apply(Cell cell)
    {
        _resources[cell] = cell.Resources;
        cell.Resources = 0;
        if (Resource is not null && Environment is not null)
            Resource?.GenerateAt(cell.Coord, cell.Layer, Environment);
    }

    public void Remove(Cell cell)
    {
        cell.Resources = _resources[cell];
        _resources.Remove(cell);
    }
}
