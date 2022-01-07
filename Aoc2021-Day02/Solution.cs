namespace Aoc2021_Day02;

internal class Solution
{
    public string Title => "Day 2: Dive!";

    public object? PartOne()
    {
        var instructions = InputFile.ReadAllLines()
                                    .Select(Instruction.Read)
                                    .ToArray();

        var horizontal = 0;
        var depth = 0;

        foreach (var (action, value) in instructions)
        {
            switch (action)
            {
                case "up":
                    depth -= value;
                    break;
                case "down":
                    depth += value;
                    break;
                case "forward":
                    horizontal += value;
                    break;
            }
        }

        return horizontal * depth;
    }

    public object? PartTwo()
    {
        var instructions = InputFile.ReadAllLines()
                                    .Select(Instruction.Read)
                                    .ToArray();

        var horizontal = 0;
        var depth = 0;
        var aim = 0;

        foreach (var (action, value) in instructions)
        {
            switch (action)
            {
                case "up":
                    aim -= value;
                    break;
                case "down":
                    aim += value;
                    break;
                case "forward":
                    horizontal += value;
                    depth += aim * value;
                    break;
            }
        }

        return horizontal * depth;
    }
}
