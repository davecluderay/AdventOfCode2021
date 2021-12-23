namespace Aoc2021_Day22;

internal class Solution
{
    public string Title => "Day 22: Reactor Reboot";

    public object? PartOne()
    {
        var initializationRegion = new Cuboid(-50, 50, -50, 50, -50, 50);
        var initialisationSteps = InputFile.ReadAllLines()
                                           .Select(RebootStep.Parse)
                                           .Where(step => initializationRegion.CompletelyContains(step.Cuboid))
                                           .ToArray();
        var turnedOnCuboids = ExecuteSteps(initialisationSteps);
        return turnedOnCuboids.Sum(c => c.CalculateCubeCount());
    }
        
    public object? PartTwo()
    {
        var rebootSteps = InputFile.ReadAllLines()
                                   .Select(RebootStep.Parse)
                                   .ToArray();
        var turnedOnCuboids = ExecuteSteps(rebootSteps);
        return turnedOnCuboids.Sum(c => c.CalculateCubeCount());
    }

    private static Cuboid[] ExecuteSteps(IEnumerable<RebootStep> steps)
    {
        var turnedOnCuboids = Enumerable.Empty<Cuboid>();
        foreach (var step in steps)
        {
            turnedOnCuboids = turnedOnCuboids.SelectMany(c => c.Subtract(step.Cuboid));
            if (step.IsOn)
            {
                turnedOnCuboids = turnedOnCuboids.Append(step.Cuboid);
            }
        }
        return turnedOnCuboids.ToArray();
    }
}