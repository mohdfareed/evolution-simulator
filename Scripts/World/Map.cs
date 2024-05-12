using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class Map : Node2D
{
    [Export] public int WorldBoundaryMargin = 4; // in tiles

    [Signal] public delegate void WorldBoundsReachedEventHandler(Node2D body);
    [Signal] public delegate void BiomeChangedEventHandler(Biome biome, Node2D body);

    private Area2D _worldMap = null!; // area of the world
    private Vector2[][] _mapShapes = Array.Empty<Vector2[]>(); // bounds of the world
    private readonly Dictionary<Biome, Area2D> _biomesAreas = new(); // areas of biomes
    private Dictionary<Biome, Vector2[][]> _biomesShapes = new(); // bounds of biomes

    public void ExpandMap(Dictionary<Vector2I, Biome> biomeMap, Func<Vector2I, Vector2> mapToGlobal)
    {

        // // combine all cells for world bounds
        // var mapBorders = MapBorders(biomeMap, mapToGlobal);
        // _mapShapes = new Vector2[][]
        // {
        //     mapBorders.Select(b =>
        //     {
        //         return b.Item1;
        //     }).ToArray()
        // };
        // UpdateArea(_worldMap, _mapShapes);
    }

    private static List<Tuple<Vector2, Vector2>> MapBorders(Dictionary<Vector2I, Biome> biomeMap, Func<Vector2I, Vector2> mapToGlobal)
    {
        // construct borders around world
        var borders = new List<Tuple<Vector2, Vector2>>();
        foreach (var coord in biomeMap.Keys)
        {
            var cell = new BiomeCell(coord, mapToGlobal, biomeMap[coord]);
            borders.AddRange(cell.WorldBorder(biomeMap));
        }

        // sort segments to form closed shapes
        var sorted = new List<Tuple<Vector2, Vector2>>();
        var current = borders[0];
        borders.RemoveAt(0);

        while (borders.Count > 0)
        {
            var next = borders.FirstOrDefault(b => b.Item1 == current.Item2);
            if (next is null)
            {
                sorted.Add(current);
                current = borders[0];
                borders.RemoveAt(0);
            }
            else
            {
                borders.Remove(next);
                if (next.Item1 == current.Item2)
                    current = new Tuple<Vector2, Vector2>(current.Item1, next.Item2);
                else
                    current = new Tuple<Vector2, Vector2>(current.Item2, next.Item1);
            }
        }

        GD.Print($"World bounds: {borders.Select(b => $"{b.Item1} - {b.Item2}").ToArray()}");
        return borders;
    }

    private static Dictionary<Biome, List<Tuple<Vector2, Vector2>>> BiomesBorders(Dictionary<Vector2I, Biome> biomeMap, Func<Vector2I, Vector2> mapToGlobal)
    {
        // construct borders around biomes
        var borders = new Dictionary<Biome, List<Tuple<Vector2, Vector2>>>();
        foreach (var coord in biomeMap.Keys)
        {
            var biome = biomeMap[coord];
            if (biome == Biome.None)
                continue;
            if (!borders.TryGetValue(biome, out var border))
                borders[biome] = new List<Tuple<Vector2, Vector2>>();

            var cell = new BiomeCell(coord, mapToGlobal, biome);
            border?.AddRange(cell.Border(biomeMap));
        }

        // sort segments to form closed shapes
        foreach (var biome in borders.Keys)
        {
            var border = borders[biome];
            var sorted = new List<Tuple<Vector2, Vector2>>();
            var current = border[0];
            border.RemoveAt(0);
            while (border.Count > 0)
            {
                var next = border.FirstOrDefault(b => b.Item1 == current.Item2 || b.Item2 == current.Item2);
                if (next is null)
                {
                    sorted.Add(current);
                    current = border[0];
                    border.RemoveAt(0);
                }
                else
                {
                    border.Remove(next);
                    if (next.Item1 == current.Item2)
                        current = new Tuple<Vector2, Vector2>(current.Item1, next.Item2);
                    else
                        current = new Tuple<Vector2, Vector2>(current.Item2, next.Item1);
                }
            }
            sorted.Add(current);
            borders[biome] = sorted;
        }
        return borders;
    }

    private void UpdateArea(Area2D area, Vector2[][] shapes)
    {
        // create missing colliders
        while (area.GetChildCount() < shapes.Length)
            area.AddChild(new CollisionPolygon2D());

        // update existing colliders
        for (int i = 0; i < shapes.Length; i++)
            if (area.GetChild(i) is CollisionPolygon2D collider)
                CallDeferred(nameof(SetCollider), collider, shapes[i], $"Shape {i}");

        // disable unused colliders
        for (int i = shapes.Length; i < area.GetChildCount(); i++)
            if (area.GetChild(i) is CollisionPolygon2D collider)
                CallDeferred(nameof(ResetCollider), collider);
    }

    private static void SetCollider(CollisionPolygon2D collider, Vector2[] shape, string name)
    {
        collider.BuildMode = CollisionPolygon2D.BuildModeEnum.Segments;
        collider.Polygon = shape;
        collider.Disabled = false;
        collider.Name = name;
    }

    private static void ResetCollider(CollisionPolygon2D collider)
    {
        collider.Polygon = Array.Empty<Vector2>();
        collider.Disabled = true;
    }

    public override void _Ready()
    {
        // create world area
        _worldMap = new Area2D()
        {
            Name = "Boundaries",
            Monitorable = false,
            Monitoring = true,
            CollisionLayer = (uint)CollisionLayer.None,
            CollisionMask = (uint)CollisionLayer.Creature,
        };
        AddChild(_worldMap);
        // create preliminary world bounds (to avoid editor warnings)
        var collider = new CollisionPolygon2D() { Name = "Shape" };
        _worldMap.AddChild(collider);
        collider.Owner = GetTree().EditedSceneRoot; // FIXME: not displaying in editor

        // create biome areas
        foreach (Biome biome in Enum.GetValues(typeof(Biome)))
        {
            var area = new Area2D()
            {
                Name = biome.ToString(),
                Monitorable = false,
                Monitoring = true,
                CollisionLayer = (uint)CollisionLayer.None,
                CollisionMask = (uint)CollisionLayer.Creature,
            };
            AddChild(area);
            _biomesAreas[biome] = area;
            _biomesShapes[biome] = Array.Empty<Vector2[]>();
            // create biome bounds
            collider = new CollisionPolygon2D() { Name = "Shape" };
            area.AddChild(collider);
            collider.Owner = GetTree().EditedSceneRoot;
        }

        // connect to world boundary event
        _worldMap.BodyExited += body => EmitSignal(SignalName.WorldBoundsReached, body);
        // connect to biome change event
        foreach (Biome biome in Enum.GetValues(typeof(Biome)))
            _biomesAreas[biome].BodyEntered += body =>
                EmitSignal(SignalName.BiomeChanged, (int)biome, body);
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();
        if (_worldMap is null)
            warnings.Add($"World area not found: {nameof(CollisionPolygon2D)}");
        foreach (var (biome, area) in _biomesAreas)
        {
            if (area is null)
                warnings.Add($"Biome area not found: {nameof(Area2D)}");
        }
        return warnings.ToArray();
    }
}

public struct BiomeCell
{
    public Vector2I Coord;
    public Func<Vector2I, Vector2> MapToGlobal;
    public Biome biome;
    public BiomeCell(Vector2I coord, Func<Vector2I, Vector2> mapToGlobal, Biome biome)
    {
        Coord = coord;
        MapToGlobal = mapToGlobal;
        this.biome = biome;
    }

    // cells
    public readonly Vector2I LeftNeighbor => Coord + Vector2I.Left;
    public readonly Vector2I RightNeighbor => Coord + Vector2I.Right;
    public readonly Vector2I UpNeighbor => Coord + Vector2I.Up;
    public readonly Vector2I DownNeighbor => Coord + Vector2I.Down;
    public readonly Vector2I[] Neighbors => new Vector2I[4] {
        LeftNeighbor,
        RightNeighbor,
        UpNeighbor,
        DownNeighbor
    };


    public readonly Vector2 TopLeftBound => MapToGlobal(new Vector2I(Coord.X, Coord.Y));
    public readonly Vector2 TopRightBound => MapToGlobal(new Vector2I(Coord.X + 1, Coord.Y));
    public readonly Vector2 BottomRightBound => MapToGlobal(new Vector2I(Coord.X + 1, Coord.Y + 1));
    public readonly Vector2 BottomLeftBound => MapToGlobal(new Vector2I(Coord.X, Coord.Y + 1));

    public readonly Vector2[] Bounds => new Vector2[4] {
        TopLeftBound,
        TopRightBound,
        BottomRightBound,
        BottomLeftBound
    };

    public Tuple<Vector2, Vector2> LeftEdge => new(TopLeftBound, BottomLeftBound);
    public Tuple<Vector2, Vector2> RightEdge => new(TopRightBound, BottomRightBound);
    public Tuple<Vector2, Vector2> TopEdge => new(TopLeftBound, TopRightBound);
    public Tuple<Vector2, Vector2> BottomEdge => new(BottomLeftBound, BottomRightBound);

    public Tuple<Vector2, Vector2>[] Border(Dictionary<Vector2I, Biome> bioMap)
    {
        // create borders around biomes
        var borders = new List<Tuple<Vector2, Vector2>>();
        if (!bioMap.TryGetValue(LeftNeighbor, out var left) || left != biome)
            borders.Add(LeftEdge);
        if (!bioMap.TryGetValue(RightNeighbor, out var right) || right != biome)
            borders.Add(RightEdge);
        if (!bioMap.TryGetValue(UpNeighbor, out var up) || up != biome)
            borders.Add(TopEdge);
        if (!bioMap.TryGetValue(DownNeighbor, out var down) || down != biome)
            borders.Add(BottomEdge);
        return borders.ToArray();
    }

    public Tuple<Vector2, Vector2>[] WorldBorder(Dictionary<Vector2I, Biome> bioMap)
    {
        // create borders around world
        var borders = new List<Tuple<Vector2, Vector2>>();
        if (!bioMap.TryGetValue(LeftNeighbor, out var left) || left == Biome.None)
            borders.Add(LeftEdge);
        if (!bioMap.TryGetValue(RightNeighbor, out var right) || right == Biome.None)
            borders.Add(RightEdge);
        if (!bioMap.TryGetValue(UpNeighbor, out var up) || up == Biome.None)
            borders.Add(TopEdge);
        if (!bioMap.TryGetValue(DownNeighbor, out var down) || down == Biome.None)
            borders.Add(BottomEdge);
        return borders.ToArray();
    }
}
