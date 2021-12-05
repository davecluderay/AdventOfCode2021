using System.Text.RegularExpressions;

namespace Aoc2021_Day05;

internal class Solution
{
    public string Title => "Day 5: Hydrothermal Venture";

    public object? PartOne()
    {
        var lines = ReadLines().Where(l => l.P1.X == l.P2.X || l.P1.Y == l.P2.Y).ToArray();
        var points = lines.SelectMany(l => GetPointsOnLine(l.P1, l.P2));
        return points.GroupBy(p => p).Count(g => g.Count() > 1);
    }

    public object? PartTwo()
    {
        var lines = ReadLines();
        var points = lines.SelectMany(l => GetPointsOnLine(l.P1, l.P2));
        return points.GroupBy(p => p).Count(g => g.Count() > 1);
    }

    private static ((int X, int Y) P1, (int X, int Y) P2)[] ReadLines()
    {
        var pattern = new Regex(@"^(?<X1>\d+),(?<Y1>\d+) -> (?<X2>\d+),(?<Y2>\d+)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        return InputFile.ReadAllLines()
                        .Select(s => pattern.Match(s))
                        .Where(m => m.Success)
                        .Select(m => ((Convert.ToInt32(m.Groups["X1"].Value), Convert.ToInt32(m.Groups["Y1"].Value)),
                                      (Convert.ToInt32(m.Groups["X2"].Value), Convert.ToInt32(m.Groups["Y2"].Value))))
                        .ToArray();
    }

    private IEnumerable<(int X, int Y)> GetPointsOnLine((int X, int Y) p1, (int X, int Y) p2)
    {
        var (x1, x2, xd) = (Math.Min(p1.X, p2.X), Math.Max(p1.X, p2.X), Math.Sign(p2.X - p1.X));
        var (y1, y2, yd) = (Math.Min(p1.Y, p2.Y), Math.Max(p1.Y, p2.Y), Math.Sign(p2.Y - p1.Y));

        var xn = x2 - x1 + 1;
        var yn = y2 - y1 + 1;

        var xs = xd == 0 ? Enumerable.Repeat(x1, yn) : xd > 0 ? Enumerable.Range(x1, xn): Enumerable.Range(x1, xn).Reverse();
        var ys = yd == 0 ? Enumerable.Repeat(y1, xn) : yd > 0 ? Enumerable.Range(y1, yn): Enumerable.Range(y1, yn).Reverse();

        return xs.Zip(ys).Select(p => (p.First, p.Second));
    }
}
