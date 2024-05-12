using System;
using System.Collections.Generic;
using System.Threading;
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
    private Node2D _boundsEffectResource = null!;
    private BorderEffect _boundsEffect = null!;
    private bool _isOnCooldown = false;

    public void ExpandMap(IEnumerable<Vector2I> generatedCells, Environment environment)
    {
        // find world borders
        foreach (var cellCoord in generatedCells)
        {
            if (environment[cellCoord] is not Cell cell)
                continue; // skip if cell is not found

            // unsubscribe from previous events
            cell.RemoveEffect(_boundsEffect);
            cell.Entered -= OnWorldBoundsReached;

            // subscribe to world bounds
            if (IsCellWorldBound(cell, environment.GetCell))
            {
                _boundsEffect.MapToGlobal = (coord) => environment.MapToGlobal(coord);
                cell.AddEffect(_boundsEffect);
                cell.Entered += OnWorldBoundsReached;
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

    private void Initialize()
    {
        foreach (var child in GetChildren())
        {
            if (child is Node2D boundsEffect)
                _boundsEffectResource = boundsEffect;
        }
    }

    public override void _Ready()
    {
        Initialize(); // initialize environment
        _boundsEffect = new()
        {
            BoundsEffect = _boundsEffectResource,
        };
        _boundsEffectResource.Visible = false;
    }

    public override string[] _GetConfigurationWarnings()
    {
        Initialize(); // initialize environment
        var warnings = new List<string>();
        if (ExpansionCoolDown <= 0)
            warnings.Add("Expansion cooldown is not valid.");
        return warnings.ToArray();
    }
}

public class BorderEffect : ICellEffect
{
    public Node2D? BoundsEffect { get; set; }
    public Func<Vector2I, Vector2> MapToGlobal { get; set; } = (coord) => Vector2.Zero;
    private readonly Dictionary<Cell, Tuple<Node2D?, float>> _resources = new();

    public void Apply(Cell cell)
    {
        // create bounds effect
        var effectInstance = BoundsEffect?.Duplicate() as Node2D;
        if (effectInstance is not null)
        {
            effectInstance.GlobalPosition = MapToGlobal(cell.Coord);
            BoundsEffect?.GetParent().AddChild(effectInstance);
            effectInstance.Visible = true;
        }
        _resources[cell] = new(effectInstance, cell.Resources);
        cell.Resources = 0;
    }

    public void Remove(Cell cell)
    {
        cell.Resources = _resources[cell].Item2;
        _resources[cell].Item1?.QueueFree();
        _resources.Remove(cell);
    }
}
