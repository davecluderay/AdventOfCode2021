namespace Aoc2021_Day07;

internal class Solution
{
    public string Title => "Day 7: The Treachery of Whales";

    public object? PartOne()
    {
        long CalculateMovementFuelCost(int distance) => distance;
        return FindLowestAlignmentFuelCost(CalculateMovementFuelCost);
    }

    public object? PartTwo()
    {
        long CalculateMovementFuelCost(int distance) => Enumerable.Range(1, distance).Sum();
        return FindLowestAlignmentFuelCost(CalculateMovementFuelCost);
    }

    private static long FindLowestAlignmentFuelCost(Func<int, long> calculateMovementFuelCost)
    {
        var initialPositions = ReadInitialPositions();

        var positionRange = Enumerable.Range(initialPositions.Min(), initialPositions.Max() - initialPositions.Min() + 1);

        // Pre-calculate the cost for each possible movement distance.
        var fuelCostsByDistance = positionRange.Select(p => p - positionRange.First())
                                               .ToDictionary(d => d, calculateMovementFuelCost);

        // Find the alignment position with the lowest fuel cost.
        return positionRange.Min(alignTo => initialPositions.Sum(p => fuelCostsByDistance[Math.Abs(p - alignTo)]));
    }

    private static int[] ReadInitialPositions()
    {
        return InputFile.ReadAllLines()
                        .SelectMany(s => s.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                        .Select(s => Convert.ToInt32(s))
                        .ToArray();
    }
}
