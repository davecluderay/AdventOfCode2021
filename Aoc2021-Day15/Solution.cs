namespace Aoc2021_Day15;

internal class Solution
{
    public string Title => "Day 15: Chiton";

    public object? PartOne()
    {
        var map = ReadMapFromFile();
        return FindLowestRiskPath(map, (0, 0), (map.GetLength(0) - 1, map.GetLength(1) - 1));
    }

    public object? PartTwo()
    {
        var map = ReadMapFromFile();
        map = ExpandMap(map, factor: 5);
        return FindLowestRiskPath(map, (0, 0), (map.GetLength(0) - 1, map.GetLength(1) - 1));
    }

    private static int[,] ReadMapFromFile()
    {
        var lines = InputFile.ReadAllLines();
        var map = new int[lines.First().Length, lines.Length];
        for(var x = 0; x < map.GetLength(0); x++)
        for (var y = 0; y < map.GetLength(1); y++)
            map[x, y] = lines[y][x] - '0';
        return map;
    }

    private static int[,] ExpandMap(int[,] original, int factor)
    {
        var newMap = new int[original.GetLength(0) * factor, original.GetLength(1) * factor];

        for (var x = 0; x < original.GetLength(0); x++)
        for (var y = 0; y < original.GetLength(1); y++)
        {
            for (var dx = 0; dx < factor; dx++)
            for (var dy = 0; dy < factor; dy++)
            {
                var newX = x + dx * original.GetLength(0);
                var newY = y + dy * original.GetLength(1);
                var newValue = (original[x, y] + dx + dy);
                while (newValue > 9) newValue -= 9;
                newMap[newX, newY] = newValue;
            }
        }

        return newMap;
    }

    private static int FindLowestRiskPath(int[,] map, (int x, int y) from, (int x, int y) to)
    {
        var visited = new HashSet<(int x, int y)>();
        var nextCandidates = new HashSet<(int x, int y)>();

        var lowestRiskPaths = new Dictionary<(int x, int y), int>
        {
            [from] = 0
        };

        var current = from;
        while (visited.Count < map.Length)
        {
            visited.Add(current);
            if (current == to) break;

            var lowestRiskToCurrent = lowestRiskPaths.GetValueOrDefault(current, int.MaxValue);

            var unvisitedNeighbours = GetNeighbourPositions(map, current).Where(p => !visited.Contains(p)).ToArray();
            foreach (var n in unvisitedNeighbours)
            {
                var risk = map[n.x, n.y];
                lowestRiskPaths[n] = Math.Min(lowestRiskPaths.GetValueOrDefault(n, int.MaxValue), lowestRiskToCurrent + risk);
                nextCandidates.Add(n);
            }

            current = nextCandidates.OrderBy(c => (long)lowestRiskPaths.GetValueOrDefault(c, int.MaxValue)).First();
            nextCandidates.Remove(current);
        }

        return lowestRiskPaths.GetValueOrDefault(to, int.MaxValue);
    }

    private static IEnumerable<(int x, int y)> GetNeighbourPositions(int[,] map, (int x, int y) position)
    {
        var (maxX, maxY) = (map.GetLength(0) - 1, map.GetLength(1) - 1);
        var (x, y) = position;

        if (x > 0) yield return (x - 1, y);
        if (x < maxX) yield return (x + 1, y);
        if (y > 0) yield return (x, y - 1);
        if ( y < maxY) yield return (x, y + 1);
    }
}
