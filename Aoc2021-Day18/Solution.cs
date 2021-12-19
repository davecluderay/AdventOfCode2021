namespace Aoc2021_Day18;

internal class Solution
{
    public string Title => "Day 18: Snailfish";

    public object? PartOne()
    {
        return InputFile.ReadAllLines()
                        .Select(Number.Parse)
                        .Aggregate((Number?)null,
                                   (sum, number) => sum is null ? number : sum + number,
                                   sum => sum?.Magnitude ?? 0L);
    }

    public object? PartTwo()
    {
        var numbers = InputFile.ReadAllLines()
                               .Select(Number.Parse)
                               .ToArray();

        var highestMagnitude = 0L;
        for (var i = 0; i < numbers.Length - 1; i++)
        for (var j = i + 1; j < numbers.Length; j++)
        {
            highestMagnitude = Math.Max(highestMagnitude, (numbers[i] + numbers[j]).Magnitude);
            highestMagnitude = Math.Max(highestMagnitude, (numbers[j] + numbers[i]).Magnitude);
        }

        return highestMagnitude;
    }
}
