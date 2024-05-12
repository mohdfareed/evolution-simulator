using Godot;

namespace EvolutionSimulator.World;

public struct Cell
{
    public Vector2I Position { get; set; }
    public Biome Biome { get; set; }
    public Resource[] Resources { get; set; }
}

public enum Biome // 4 basic biomes
{
    Ocean, // cold, wet, water
    Desert, // hot, dry, sandy
    Forest, // hot, wet, green
    Mountain, // cold, dry, rocky
    None // biome agnostic
}
