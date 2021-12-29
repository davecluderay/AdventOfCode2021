namespace Aoc2021_Day25;

internal class Solution
{
    public string Title => "Day 25: Sea Cucumber";

    public object? PartOne()
    {
        var map = InputFile.ReadAllLines()
                           .SelectMany((line, y) => line.Select((c, x) => KeyValuePair.Create((X: x, Y: y), c)))
                           .ToDictionary(e => e.Key, e => e.Value);
        var (upperX, upperY) = (map.Keys.Max(k => k.X) + 1, map.Keys.Max(k => k.Y) + 1);

        var steps = 0;
        while (true)
        {
            ++steps;

            // Move any unblocked east-facing sea cucumbers.
            var eastMovements = new List<Action>();
            foreach (var (position, _) in map.Where(e => e.Value == '>'))
            {
                var next = (X: (position.X + 1) % upperX, position.Y);
                if (map[next] == '.')
                {
                    eastMovements.Add(() =>
                                   {
                                       map[position] = '.';
                                       map[next] = '>';
                                   });
                }
            }

            foreach (var action in eastMovements)
            {
                action.Invoke();
            }

            // Move any unblocked south-facing sea cucumbers.
            var southMovements = new List<Action>();
            foreach (var (position, _) in map.Where(e => e.Value == 'v'))
            {
                var next = (position.X, Y: (position.Y + 1) % upperY);
                if (map[next] == '.')
                {
                    southMovements.Add(() =>
                                       {
                                           map[position] = '.';
                                           map[next] = 'v';
                                       });
                }
            }

            foreach (var action in southMovements)
            {
                action.Invoke();
            }

            if (!eastMovements.Any() && !southMovements.Any())
            {
                return steps;
            }
        }
    }

    public object? PartTwo()
    {
        return "Merry Christmas!";
    }
}
