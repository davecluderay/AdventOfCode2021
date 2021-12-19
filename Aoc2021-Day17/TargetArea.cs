using System.Text.RegularExpressions;

namespace Aoc2021_Day17;

internal record TargetArea(int MinX, int MaxX, int MinY, int MaxY)
{
    public static TargetArea ReadFromFile()
    {
        var match = InputFile.ReadAllLines()
                             .Select(line => Regex.Match(line, @"^target area: x=(?<MinX>-?\d+)\.\.(?<MaxX>-?\d+), y=(?<MinY>-?\d+)\.\.(?<MaxY>-?\d+)"))
                             .Single(m => m.Success);

        var area = new TargetArea(Convert.ToInt32(match.Groups["MinX"].Value),
                                  Convert.ToInt32(match.Groups["MaxX"].Value),
                                  Convert.ToInt32(match.Groups["MinY"].Value),
                                  Convert.ToInt32(match.Groups["MaxY"].Value));

        if (area.MinX < 0 || area.MaxX < 0) throw new NotSupportedException("Solution assumes the target area is in the positive x-axis.");
        if (area.MinY > 0 || area.MaxY > 0) throw new NotSupportedException("Solution assumes the target area is in the negative y-axis.");

        return area;
    }
}
