namespace Aoc2021_Day09;

internal class Solution
{
    public string Title => "Day 9: Smoke Basin";

    public object? PartOne()
    {
        var map = HeightMap.LoadFromInputFile();
        var lowPoints = map.FindLowPoints();
        return lowPoints.Sum(p => map.GetHeightAt(p) + 1);
    }

    public object? PartTwo()
    {
        var map = HeightMap.LoadFromInputFile();
        var lowPoints = map.FindLowPoints();
        var basinSizes = lowPoints.Select(map.FindBasinSize);
        return basinSizes.OrderByDescending(s => s)
                         .Take(3)
                         .Aggregate(1, (product, value) => product * value);
    }
}
