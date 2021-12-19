namespace Aoc2021_Day17;

internal class Solution
{
    public string Title => "Day 17: Trick Shot";

    public object? PartOne()
    {
        var targetArea = TargetArea.ReadFromFile();
        var velocities = FindValidInitialVelocities(targetArea);
        var best = velocities.OrderByDescending(v => v.Y).First();
        var maximumHeight = Enumerable.Range(0, best.Y)
                                      .Aggregate(0, (a, v) => a + (best.Y - v));
        return maximumHeight;
    }

    public object? PartTwo()
    {
        var targetArea = TargetArea.ReadFromFile();
        var velocities = FindValidInitialVelocities(targetArea);
        return velocities.Length;
    }

    private static (int X, int Y)[] FindValidInitialVelocities(TargetArea targetArea)
    {
        var (x1, x2, y1, y2) = targetArea;

        // Find any initial x-velocities that put the probe in the target area after any step.
        var initialXVelocities = Enumerable.Range(1, x2) // An initial velocity higher than x2 would immediately overshoot.
                                           .Where(initialVelocity =>
                                                      Enumerable.Range(1, initialVelocity) // No more X movement beyond this step.
                                                                .Select(step => CalculateXPositionAfter(step, initialVelocity))
                                                                .Any(x => x >= x1 && x <= x2))
                                           .ToArray();

        // Find corresponding initial y-velocities that put the probe in the target area after any step for each of the identified x-velocities.
        var validInitialVelocities = new List<(int x, int y)>();

        foreach (var ixv in initialXVelocities)
        {
            // Start with the lowest y-velocity that could put the probe in the target area.
            // Finish with the highest y-velocity that could put the probe in the target area.
            for (var iyv = Math.Min(y1, y2); iyv <= Math.Max(Math.Abs(y1), Math.Abs(y2)); iyv++)
            {
                var step = 1;
                while (true)
                {
                    var (x, y) = (CalculateXPositionAfter(step, ixv), CalculateYPositionAfter(step, iyv));

                    if (x > x2 || y < y1) break; // Overshot.

                    if (x >= x1 && y <= y2)
                    {
                        // In the target area.
                        validInitialVelocities.Add((ixv, iyv));
                        break;
                    }

                    step++;
                }
            }
        }

        return validInitialVelocities.ToArray();
    }

    private static int CalculateXPositionAfter(int steps, int initialVelocity)
    {
        initialVelocity = Math.Max(initialVelocity, 0);
        steps = Math.Min(initialVelocity, steps); // Position doesn't change after velocity reaches zero.
        var max = initialVelocity * (initialVelocity + 1) / 2;
        var remaining = (initialVelocity - steps) * (initialVelocity - steps + 1) / 2;
        return max - remaining;
    }

    private static int CalculateYPositionAfter(int steps, int initialVelocity)
    {
        var max = initialVelocity * (initialVelocity + 1) / 2;
        var remaining = (initialVelocity - steps) * (initialVelocity - steps + 1) / 2;
        return max - remaining;
    }
}
