using System.Collections.Generic;
using Godot;

namespace EvolutionSimulator.World;
public partial class Cell : RefCounted
{
    // properties
    public Vector2I Coord { get; set; } // map coordinates
    public Biome Biome { get; set; }
    public float Resources { get; set; }
    public EnvironmentLayer Layer { get; set; }

    // effects
    private List<ICellEffect> Effects { get; set; } = new();
    public void AddEffect(ICellEffect effect)
    {
        if (Effects.Contains(effect))
            return;
        Effects.Add(effect);
        effect.Apply(this);
    }
    public void RemoveEffect(ICellEffect effect)
    {
        if (Effects.Remove(effect))
            effect.Remove(this);
    }

    // events
    [Signal] public delegate void EnteredEventHandler(Cell cell, Node body);
    [Signal] public delegate void ExitedEventHandler(Cell cell, Node body);
    public void Enter(Node body) => EmitSignal(SignalName.Entered, this, body);
    public void Exit(Node body) => EmitSignal(SignalName.Exited, this, body);

    // navigation
    public Vector2I LeftNeighbor(int i = 1) => Coord + Vector2I.Left * i;
    public Vector2I RightNeighbor(int i = 1) => Coord + Vector2I.Right * i;
    public Vector2I UpNeighbor(int i = 1) => Coord + Vector2I.Up * i;
    public Vector2I DownNeighbor(int i = 1) => Coord + Vector2I.Down * i;
    public Vector2I UpLeftNeighbor(int i = 1) => Coord + (Vector2I.Up + Vector2I.Left) * i;
    public Vector2I UpRightNeighbor(int i = 1) => Coord + (Vector2I.Up + Vector2I.Right) * i;
    public Vector2I DownLeftNeighbor(int i = 1) => Coord + (Vector2I.Down + Vector2I.Left) * i;
    public Vector2I DownRightNeighbor(int i = 1) => Coord + (Vector2I.Down + Vector2I.Right) * i;
    public Vector2I[] Neighbors(int i = 1) => new Vector2I[8] {
        LeftNeighbor(i),
        RightNeighbor(i),
        UpNeighbor(i),
        DownNeighbor(i),
        UpLeftNeighbor(i),
        UpRightNeighbor(i),
        DownLeftNeighbor(i),
        DownRightNeighbor(i)
    };

    // operators
    public static bool operator ==(Cell? a, Cell? b) => a?.Coord == b?.Coord;
    public static bool operator !=(Cell? a, Cell? b) => a?.Coord != b?.Coord;
    public override bool Equals(object? obj) => obj is Cell cell && cell == this;
    public override int GetHashCode() => Coord.GetHashCode();
    public override string ToString() => $"Cell({Coord})";
}

public interface ICellEffect
{
    void Apply(Cell cell);
    void Remove(Cell cell);
}

public enum EnvironmentLayer
{
    Ground, // ground on which entities can walk
    Surface, // surface on which entities can interact
    Air, // air in which entities can fly
}

public enum Biome // 4 basic biomes
{
    Ocean, // cold, wet, water
    Desert, // hot, dry, sandy
    Forest, // hot, wet, green
    Mountain, // cold, dry, rocky
    None // biome agnostic
}
