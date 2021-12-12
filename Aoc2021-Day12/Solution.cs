using System.Collections.Concurrent;

namespace Aoc2021_Day12;

internal class Solution
{
    public string Title => "Day 12: Passage Pathing";

    public object? PartOne()
    {
        var caves = ReadCavesFromFile();
        var start = caves.Single(c => c.IsStart);
        var paths = FindPaths(start);
        return paths.Count();
    }

    public object? PartTwo()
    {
        var caves = ReadCavesFromFile();
        var start = caves.Single(c => c.IsStart);
        var allPaths = caves.Where(c => c.IsSmallCave)
                            .SelectMany(c => FindPaths(start, allowTwoVisitsTo: c));

        return allPaths.Select(p => string.Join(",", p.Select(c => c.Name)))
                       .Distinct()
                       .Count();
    }

    private static Cave[] ReadCavesFromFile()
    {
        var caves = new ConcurrentDictionary<string, Cave>();

        var pairs = InputFile.ReadAllLines()
                             .Select(line => line.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        foreach (var pair in pairs)
        {
            var cave1 = caves.GetOrAdd(pair[0], n => new Cave(n));
            var cave2 = caves.GetOrAdd(pair[1], n => new Cave(n));
            cave1.Connections.Add(cave2);
            cave2.Connections.Add(cave1);
        }

        return caves.Values.ToArray();
    }

    private static IEnumerable<IEnumerable<Cave>> FindPaths(Cave current, Cave allowTwoVisitsTo = null, IEnumerable<Cave>? visited = null)
    {
        visited ??= Enumerable.Empty<Cave>();

        foreach (var next in current.Connections)
        {
            if (next.IsStart) continue;

            if (next.IsSmallCave && visited.Count(c => c == next) >= (next == allowTwoVisitsTo ? 2 : 1))
            {
                // Already visited this small cave the maximum allowed times.
                continue;
            }

            if (next.IsEnd)
            {
                yield return visited.Append(current).Append(next);
                continue;
            }

            foreach (var path in FindPaths(next, allowTwoVisitsTo, visited.Append(current)))
            {
                yield return path;
            }
        }
    }
}
