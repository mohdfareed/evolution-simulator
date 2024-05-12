using System;
using System.Collections.Generic;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class RandomResource : WorldResource
{
    [Export] public WorldResource?[] Resources { get; set; } = Array.Empty<WorldResource>();
    [Export] public float[] Weights = Array.Empty<float>();

    private float total = 0;

    public RandomResource()
    {
        if (Resources.Length != Weights.Length)
            Array.Resize(ref Weights, Resources.Length);

        foreach (var weight in Weights)
            total += weight;
    }

    public override void GenerateAt(Vector2I position, EnvironmentLayer layer, TileMap tilemap)
    {
        var random = GD.Randf();
        for (var i = 0; i < Resources.Length; i++)
        {
            random -= Weights[i] / total;
            if (random <= 0)
            {
                Resources[i]?.GenerateAt(position, layer, tilemap);
                return;
            }
        }
    }

    public override IEnumerable<string> Warnings(TileMap tilemap)
    {
        if (Resources.Length != Weights.Length)
            yield return $"{this}: Resources and Weights length mismatch.";
        foreach (var weight in Weights)
            if (weight < 0)
                yield return $"{this}: Weight is negative.";

        foreach (var resource in Resources)
            if (resource is null)
                yield return $"{this}: Resource is null.";
            else
                foreach (var warning in resource.Warnings(tilemap))
                    yield return $"{this}: {warning}";
    }
}
