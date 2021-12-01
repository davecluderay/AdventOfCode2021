namespace Aoc2021_Day01;

internal class Solution
{
    public string Title => "Day 1: Sonar Sweep";

    public object? PartOne()
    {
        var depths = ReadDepths();
        return CountIncreases(depths);
    }
        
    public object? PartTwo()
    {
        var depths = ReadDepths();
        return CountIncreases(depths, windowSize: 3);
    }

    private int CountIncreases(int[] depths, int windowSize = 1)
    {
        var increases = 0;

        var previous = depths.Take(windowSize).Sum();
        for (var i = windowSize; i < depths.Length; i++)
        {
            var next = previous - depths[i - windowSize] + depths[i];
            if (next > previous) increases++;
            previous = next;
        }

        return increases;
    }

    private int[] ReadDepths()
        => InputFile.ReadAllLines()
                    .Select(s => Convert.ToInt32(s))
                    .ToArray();
}
