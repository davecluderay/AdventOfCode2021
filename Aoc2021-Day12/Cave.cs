namespace Aoc2021_Day12;

internal record Cave(string Name)
{
    public bool IsStart => Name == "start";
    public bool IsEnd => Name == "end";
    public bool IsSmallCave => !IsStart && !IsEnd && Name.Any(char.IsLower);
    public HashSet<Cave> Connections { get; } = new ();
}
