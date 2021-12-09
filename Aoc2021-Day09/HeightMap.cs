namespace Aoc2021_Day09;

internal class HeightMap
{
    private readonly int[,] _data;

    private HeightMap(string[] data)
    {
        _data = new int[data.First().Length, data.Length];

        for (var y = 0; y < _data.GetLength(1); y++)
        for (var x = 0; x < _data.GetLength(0); x++)
        {
            _data[x, y] = data[y][x] - '0';
        }
    }

    public IEnumerable<(int x, int y)> FindLowPoints()
    {
        for (var y = 0; y < _data.GetLength(1); y++)
        for (var x = 0; x < _data.GetLength(0); x++)
        {
            var value = GetHeightAt((x, y));
            var neighbours = GetNeighbouringPoints((x, y));
            if (neighbours.All(n => GetHeightAt(n) > value))
                yield return (x, y);
        }
    }

    public int FindBasinSize((int x, int y) lowPoint)
    {
        // Probe outwards from the low point, stopping at level 9 positions.
        var included = new HashSet<(int x, int y)>();
        var candidates = new Stack<(int x, int y)>();
        candidates.Push(lowPoint);
        while (candidates.Count > 0)
        {
            var c = candidates.Pop();
            if (GetHeightAt(c) == 9) continue;

            included.Add(c);
            foreach (var n in GetNeighbouringPoints(c))
                if (!included.Contains(n))
                    candidates.Push(n);
        }

        return included.Count;
    }

    public int GetHeightAt((int x, int y) point)
    {
        var (x, y) = point;
        return _data[x, y];
    }

    private IEnumerable<(int x, int y)> GetNeighbouringPoints((int x, int y) point)
    {
        var (x, y) = point;
        if (y > 0) yield return (x, y - 1);
        if (y < _data.GetLength(1) - 1) yield return (x, y + 1);
        if (x > 0) yield return (x -1 , y);
        if (x < _data.GetLength(0) - 1) yield return (x + 1, y);
    }

    public static HeightMap LoadFromInputFile()
    {
        return new HeightMap(InputFile.ReadAllLines());
    }
}
