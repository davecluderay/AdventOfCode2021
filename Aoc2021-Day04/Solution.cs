namespace Aoc2021_Day04;

internal class Solution
{
    public string Title => "Day 4: Giant Squid";

    public object? PartOne()
    {
        var (draws, boards) = ReadDrawSequenceAndBoards();

        foreach (var draw in draws)
        foreach (var board in boards)
        {
            board.Mark(draw);
            if (board.IsAnyLineOrColumnComplete())
            {
                return board.GetUnmarkedNumbers().Sum() * draw;
            }
        }

        return null;
    }

    public object? PartTwo()
    {
        var (draws, boards) = ReadDrawSequenceAndBoards();

        var winningBoards = new HashSet<BingoBoard>();

        foreach (var draw in draws)
        foreach (var board in boards)
        {
            if (winningBoards.Contains(board)) continue;

            board.Mark(draw);
            if (!board.IsAnyLineOrColumnComplete()) continue;

            winningBoards.Add(board);
            if (winningBoards.Count == boards.Length)
            {
                return board.GetUnmarkedNumbers().Sum() * draw;
            }
        }

        return null;
    }

    private (int[] DrawSequence, BingoBoard[] Boards) ReadDrawSequenceAndBoards()
    {
        var lines = InputFile.ReadAllLines();
        var drawSequence = lines.First()
                                .Split(',', StringSplitOptions.TrimEntries)
                                .Select(s => Convert.ToInt32(s))
                                .ToArray();
        var boards = BingoBoard.ReadAll(lines.Skip(1)
                                             .ToArray());
        return (drawSequence, boards);
    }
}
