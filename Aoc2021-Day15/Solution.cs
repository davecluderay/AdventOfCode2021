using System.Collections.Immutable;

namespace Aoc2021_Day15;

internal class Solution
{
    public string Title => "Day 15: Chiton";

    public object? PartOne()
    {
        var map = ReadMapFromFile();
        return FindLowestRiskPath(map);
    }

    public object? PartTwo()
    {
        var map = ReadMapFromFile();
        map = ExpandMap(map, factor: 5);
        return FindLowestRiskPath(map);
    }

    private static Map ReadMapFromFile()
    {
        var tiles = InputFile.ReadAllLines()
                             .SelectMany((line, y) => line.Select((@char, x) => new MapTile((x, y), @char - '0')))
                             .ToImmutableArray();
        return new Map(tiles, tiles.First(), tiles.Last());
    }

    private static Map ExpandMap(Map map, int factor)
    {
        var width = map.End.Position.X + 1;
        var height = map.End.Position.Y + 1;

        var tiles = new List<MapTile>(map.Tiles.Count * factor * factor);
        foreach (var ((x, y), risk) in map.Tiles)
        {
            for (var dx = 0; dx < factor; dx++)
            for (var dy = 0; dy < factor; dy++)
            {
                var newX = x + dx * width;
                var newY = y + dy * height;
                var newValue = risk + dx + dy;
                while (newValue > 9) newValue -= 9;
                tiles.Add(new MapTile((newX, newY), newValue));
            }
        }

        return new Map(tiles, tiles.First(), tiles.Last());
    }

    private static int FindLowestRiskPath(Map map)
    {
        var from = map.Start.Position;
        var to = map.End.Position;

        var maxX = map.Tiles.Max(t => t.Position.X);
        var maxY = map.Tiles.Max(t => t.Position.Y);

        var nodes = map.Tiles.ToDictionary(t => t.Position);

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

    private record Map(IReadOnlyCollection<MapTile> Tiles, MapTile Start, MapTile End);
    private record MapTile((int X, int Y) Position, int Risk = 1);
}