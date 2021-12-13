using System.Text.RegularExpressions;

namespace Aoc2021_Day13;

internal class Solution
{
    public string Title => "Day 13: Transparent Origami";

    public object? PartOne()
    {
        var (dots, folds) = ReadData();
        dots = PerformFold(dots, folds.First());
        return dots.Length;
    }

    public object? PartTwo()
    {
        var (dots, folds) = ReadData();
        dots = folds.Aggregate(dots, PerformFold);
        return Render(dots); // PFKLKCFP
    }

    private static ((int X, int Y)[] Dots, (char Axis, int Offset)[] Folds) ReadData()
    {
        var dotPattern = new Regex(@"^(?<X>\d+),(?<Y>\d+)$", RegexOptions.Compiled | RegexOptions.Singleline);
        var foldPattern = new Regex(@"^fold along (?<Axis>x|y)=(?<Offset>\d+)$", RegexOptions.Compiled | RegexOptions.Singleline);

        var lines = InputFile.ReadAllLines();
        var dots = lines.Select(l => dotPattern.Match(l))
                        .Where(m => m.Success)
                        .Select(m => (Convert.ToInt32(m.Groups["X"].Value), Convert.ToInt32(m.Groups["Y"].Value)))
                        .ToArray();
        var folds = lines.Select(l => foldPattern.Match(l))
                         .Where(m => m.Success)
                         .Select(m => (m.Groups["Axis"].Value.Single(), Convert.ToInt32(m.Groups["Offset"].Value)))
                         .ToArray();
        return (dots, folds);
    }

    private static (int X, int Y)[] PerformFold((int X, int Y)[] dots, (char Axis, int Offset) fold)
    {
        var folded = new HashSet<(int X, int Y)>(dots.Length);
        foreach (var (x, y) in dots)
        {
            folded.Add(fold.Axis switch
                       {
                           'x' => x < fold.Offset ? (x, y) : (fold.Offset * 2 - x, y),
                           'y' => y < fold.Offset ? (x, y) : (x, fold.Offset * 2 - y),
                           _ => throw new InvalidOperationException($"Unexpected axis: {fold.Axis}")
                       });
        }
        return folded.ToArray();
    }

    private static string Render((int X, int Y)[] dots)
    {
        var maxX = dots.Max(d => d.X);
        var maxY = dots.Max(d => d.Y);

        var dotsLookup = dots.ToHashSet();

        var result = new StringWriter();
        for (var y = 0; y <= maxY; y++)
        {
            result.WriteLine();
            for (var x = 0; x <= maxX; x++)
            {
                result.Write(dotsLookup.Contains((x, y)) ? '#' : ' ');
            }
        }

        return result.ToString();
    }
}
