namespace Aoc2021_Day11;

internal class Solution
{
    public string Title => "Day 11: Dumbo Octopus";

    public object? PartOne()
    {
        var octopuses = OctopusMap.ReadFromFile();

        return Enumerable.Range(1, 100)
                         .Sum(_ => SimulateStep(octopuses).FlashedPositions.Length);
    }

    public object? PartTwo()
    {
        var octopuses = OctopusMap.ReadFromFile();

        var step = 1;
        while (!SimulateStep(octopuses).AreSynchronised)
            step++;

        return step;
    }

    private SimulationStepResult SimulateStep(OctopusMap octopuses)
    {
        var flashedPositions = new HashSet<(int x, int y)>();

        // First, increase the energy level of all octopuses.
        octopuses.IncreaseAllEnergyLevels();

        while (true)
        {
            // Any octopus with sufficient energy flashes, increasing the energy of its neighbours.
            var toFlash = octopuses.FindFullyEnergisedPositions()
                                   .Where(p => !flashedPositions.Contains(p))
                                   .ToArray();
            foreach (var position in toFlash)
            {
                flashedPositions.Add(position);
                octopuses.IncreaseAdjacentEnergyLevels(position);
            }

            // Repeat until all octopuses have flashed (or none of those remaining have enough energy).
            if (!toFlash.Any()) break;
        }

        // Clear the energy levels of any flashed octopuses.
        foreach (var position in flashedPositions)
        {
            octopuses.ClearEnergyLevelAt(position);
        }

        // If all octopuses have zero energy, they have synchronised.
        var synchronised = octopuses.GetAllPositions()
                                    .All(p => octopuses.GetEnergyLevelAt(p) == 0);

        return new SimulationStepResult(flashedPositions.ToArray(), synchronised);
    }

    private record SimulationStepResult((int X, int Y)[] FlashedPositions,
                                        bool AreSynchronised);

}
