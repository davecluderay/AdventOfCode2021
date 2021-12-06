namespace Aoc2021_Day06;

internal class Solution
{
    public string Title => "Day 6: Lanternfish";

    public object? PartOne()
    {
        var initialTimers = InputFile.ReadAllText()
                                     .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(s => Convert.ToInt32(s));
        return SimulateFishPopulation(initialTimers, 80);
    }

    public object? PartTwo()
    {
        var initialTimers = ReadInitialTimers();
        return SimulateFishPopulation(initialTimers, 256);
    }

    private static IEnumerable<int> ReadInitialTimers()
    {
        return InputFile.ReadAllText()
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => Convert.ToInt32(s));
    }

    private static long SimulateFishPopulation(IEnumerable<int> initialTimers, int numberOfDays)
    {
        var countsByTimer = Enumerable.Range(0, 9)
                                      .ToDictionary(n => n, _ => 0L);
        foreach (var timer in initialTimers)
        {
            countsByTimer[timer]++;
        }

        for (var day = 0; day < numberOfDays; day++)
        {
            var zeroCount = countsByTimer[0];
            for (var timer = 0; timer < 8; timer++)
            {
                countsByTimer[timer] = countsByTimer[timer + 1];
            }

            countsByTimer[8] = zeroCount;
            countsByTimer[6] += zeroCount;
        }

        return countsByTimer.Sum(f => f.Value);
    }
}
