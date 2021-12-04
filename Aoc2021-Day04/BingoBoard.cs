namespace Aoc2021_Day04;

internal class BingoBoard
{
    private readonly int[][] _lines;
    private readonly HashSet<int> _marked = new();

    private BingoBoard(IEnumerable<int[]> lines)
        => _lines = lines.ToArray();

    public void Mark(int draw)
        => _marked.Add(draw);

    public bool IsAnyLineOrColumnComplete()
        => IsAnyLineComplete() || IsAnyColumnComplete();

    public IEnumerable<int> GetUnmarkedNumbers()
        => _lines.SelectMany(l => l).Where(number => !_marked.Contains(number));

    public static BingoBoard[] ReadAll(string[] lines)
    {
        var boards = new List<BingoBoard>();
        var last = Read(lines, 0);
        while (last != null)
        {
            boards.Add(last.Value.Board);
            last = Read(lines, last.Value.nextIndex);
        }

        return boards.ToArray();
    }

    private bool IsAnyLineComplete()
        => _lines.Any(l => l.All(_marked.Contains));

    private bool IsAnyColumnComplete()
        => Enumerable.Range(0, _lines.First().Length)
                     .Any(column => _lines.Select(l => l[column]).All(_marked.Contains));

    private static (BingoBoard Board, int nextIndex)? Read(string[] lines, int startIndex)
    {
        var index = startIndex;

        // Skip blank lines and detect end.
        while (index < lines.Length && lines[index].Length == 0) index++;
        if (index == lines.Length) return null;

        // Read non-blank lines
        var data = new List<int[]>();
        while (index < lines.Length && lines[index].Length > 0)
        {
            data.Add(lines[index++].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(s => Convert.ToInt32(s))
                                   .ToArray());
        }

        return (new BingoBoard(data), index);
    }
}
