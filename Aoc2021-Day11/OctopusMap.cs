namespace Aoc2021_Day11;

internal class OctopusMap
{
    private readonly int[,] _map;

    private OctopusMap(string[] lines)
    {
        _map = new int[lines.First().Length, lines.Length];

        for (var y = 0; y < _map.GetLength(1); y++)
        for (var x = 0; x < _map.GetLength(0); x++)
        {
            _map[x, y] = lines[y][x] - '0';
        }
    }

    public int GetEnergyLevelAt((int X, int Y) position)
        => _map[position.X, position.Y];

    public void ClearEnergyLevelAt((int X, int Y) position)
        => _map[position.X, position.Y] = 0;

    public void IncreaseAllEnergyLevels()
    {
        foreach (var (x, y) in GetAllPositions())
        {
            _map[x, y]++;
        }
    }

    public void IncreaseAdjacentEnergyLevels((int X, int Y) position)
    {
        foreach (var (x, y) in GetAdjacentPositions(position))
        {
            _map[x, y]++;
        }
    }

    public IEnumerable<(int X, int Y)> GetAllPositions()
    {
        for (var y = 0; y < _map.GetLength(1); y++)
        for (var x = 0; x < _map.GetLength(0); x++)
        {
            yield return (x, y);
        }
    }

    private IEnumerable<(int X, int Y)> GetAdjacentPositions((int X, int Y) position)
    {
        var (x, y) = position;
        var minX = x > 0 ? x - 1 : x;
        var maxX = x < _map.GetLength(0) - 1 ? x + 1 : x;
        var minY = y > 0 ? y - 1 : y;
        var maxY = y < _map.GetLength(1) - 1 ? y + 1 : y;

        for (var ay = minY; ay <= maxY; ay++)
        for (var ax = minX; ax <= maxX; ax++)
        {
            if (ax != x || ay != y)
                yield return (ax, ay);
        }
    }

    public IEnumerable<(int X, int Y)> FindFullyEnergisedPositions()
    {
        for (var y = 0; y < _map.GetLength(1); y++)
        for (var x = 0; x < _map.GetLength(0); x++)
        {
            if (_map[x, y] > 9)
                yield return (x, y);
        }
    }

    public static OctopusMap ReadFromFile()
        => new (InputFile.ReadAllLines());
}
