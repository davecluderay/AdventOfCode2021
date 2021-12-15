namespace Aoc2021_Day15;

internal class Solution
{
    public string Title => "Day 15: Chiton";

    public object? PartOne()
    {
        var map = ReadMapFromFile();
        return FindLowestRiskPath(map, from: (0, 0), to: (map.Max(t => t.Position.X), map.Max(t => t.Position.Y)));
    }

    public object? PartTwo()
    {
        var map = ReadMapFromFile();
        map = ExpandMap(map, factor: 5);
        return FindLowestRiskPath(map, (0, 0), (map.Max(t => t.Position.X), map.Max(t => t.Position.Y)));
    }

    private static IReadOnlyCollection<MapTile> ReadMapFromFile()
        => InputFile.ReadAllLines()
                    .SelectMany((line, y) => line.Select((@char, x) => new MapTile((x, y), @char - '0')))
                    .ToArray();

    private static IReadOnlyCollection<MapTile> ExpandMap(IReadOnlyCollection<MapTile> map, int factor)
    {
        var width = map.Max(t => t.Position.X) + 1;
        var height = map.Max(t => t.Position.Y) + 1;

        var newMap = new List<MapTile>(map.Count * factor * factor);
        foreach (var ((x, y), risk) in map)
        {
            for (var dx = 0; dx < factor; dx++)
            for (var dy = 0; dy < factor; dy++)
            {
                var newX = x + dx * width;
                var newY = y + dy * height;
                var newValue = risk + dx + dy;
                while (newValue > 9) newValue -= 9;
                newMap.Add(new MapTile((newX, newY), newValue));
            }
        }

        return newMap.AsReadOnly();
    }

    private static int FindLowestRiskPath(IReadOnlyCollection<MapTile> map, (int x, int y) from, (int x, int y) to)
    {
        var maxX = map.Max(t => t.Position.X);
        var maxY = map.Max(t => t.Position.Y);
        var nodes = map.ToDictionary(t => t.Position);

        var lowestRiskPaths = new Dictionary<(int x, int y), int>
        {
            [from] = 0
        };

        var next = new PriorityQueue<MapTile, int>();
        next.Enqueue(nodes[from], 0);

        while (next.Count > 0)
        {
            var current = next.Dequeue();
            
            if (current.Position == to) break;

            foreach (var v in GetNeighbourPositions(current.Position))
            {
                var totalRisk = lowestRiskPaths[current.Position] + nodes[v].Risk;
                if (totalRisk < lowestRiskPaths.GetValueOrDefault(v, int.MaxValue))
                {
                    lowestRiskPaths[v] = totalRisk;
                    next.Enqueue(nodes[v], totalRisk);
                }
            }
        }

        return lowestRiskPaths[to];
        
        IEnumerable<(int x, int y)> GetNeighbourPositions((int x, int y) position)
                                       {
                                           var (x, y) = position;
                                           if (x > 0) yield return (x - 1, y);
                                           if (x < maxX) yield return (x + 1, y);
                                           if (y > 0) yield return (x, y - 1);
                                           if ( y < maxY) yield return (x, y + 1);
                                       }
    }

    private record MapTile((int X, int Y) Position, int Risk = 1);
}