using System.Linq;
using Godot;

namespace Scripts.World;
[Tool]
[GlobalClass]
public partial class Generator : Resource
{
    [Export]
    public FastNoiseLite Noise = new();
    [Export]
    public Godot.Collections.Array<GenerationSpec> Specs = new();

    public Generator()
    {
        Noise.Seed = (int)GD.Randi();
    }

    public GeneratedResource?[] GenerateAt(int x, int y)
    {
        var resources = new Godot.Collections.Array<GeneratedResource?>();
        float noise = Noise.GetNoise2D(x, y);
        foreach (GenerationSpec spec in Specs)
        {
            if (noise >= spec.Min && noise <= spec.Max)
            {
                resources.Add(spec.Resource);
            }
        }
        return resources.ToArray();
    }
}
