using System.Collections.Immutable;

namespace Aoc2021_Day23;

public class AmphipodBurrow
{
    private readonly ImmutableDictionary<(int X, int Y), char> _map;
    private readonly ((int X, int Y) Position, char Type)[] _amphipods;

    private AmphipodBurrow(ImmutableDictionary<(int X, int Y), char> map)
    {
        _map = map;
        _amphipods = map.Where(a => a.Value is 'A' or 'B' or 'C' or 'D')
                        .Select(a => (a.Key, a.Value))
                        .OrderBy(p => p)
                        .ToArray();
    }

    public ((int X, int Y) Position, char Type)[] GetAmphipods()
        => _amphipods;

    public AmphipodBurrow MoveAmphipod((int X, int Y) from, (int X, int Y) to)
        => new(_map.SetItems(new[]
                             {
                                 KeyValuePair.Create(@from, '.'),
                                 KeyValuePair.Create(to, _map[@from]),
                             }));

    public bool IsSpace((int X, int Y) position)
        => _map[position] == '.';

    public bool IsHallway((int X, int Y) position)
        => position.Y == 1 && position.X is > 0 and < 12;

    public bool IsRoomEntrance((int X, int Y) position)
        => position.Y is 1 &&
           position.X is 3 or 5 or 7 or 9;

    public bool IsRoomFor((int X, int Y) position, char amphipodType)
        => GetRoomPositions(amphipodType).Contains(position);

    public bool IsRoomClearOfOtherAmphipodTypes(char amphipodType)
        => GetRoomPositions(amphipodType).All(p => _map[p] == '.' || _map[p] == amphipodType);

    public bool IsFurthestSpaceFromRoomEntrance((int X, int Y) position)
        => Enumerable.Range(2, _amphipods.Length / 4)
                     .Reverse()
                     .First(y => _map[(position.X, y)] == '.')
           == position.Y;

    public IEnumerable<((int X, int Y) Position, int NumberOfSteps)> FindReachableSpaces((int X, int Y) from)
    {
        var visited = new HashSet<(int X, int Y)>();
        var candidates = new Stack<((int X, int Y) Position, int NunberOfSteps)>(GetAdjacentSpaces(from).Select(p => (p, 1)));
        while (candidates.Count > 0)
        {
            var candidate = candidates.Pop();
            if (visited.Contains(candidate.Position)) continue;
            visited.Add(candidate.Position);
            foreach (var space in GetAdjacentSpaces(candidate.Position))
                candidates.Push((space, candidate.NunberOfSteps + 1));
            yield return candidate;
        }
    }

    private IEnumerable<(int X, int Y)> GetAdjacentSpaces((int X, int Y) position)
    {
        var (x, y) = position;
        if (IsSpace((x - 1, y))) yield return (x - 1, y);
        if (IsSpace((x + 1, y))) yield return (x + 1, y);
        if (IsSpace((x, y - 1))) yield return (x, y - 1);
        if (IsSpace((x, y + 1))) yield return (x, y + 1);
    }

    private IEnumerable<(int X, int Y)> GetRoomPositions(char amphipodType)
    {
        var x = 3 + 2 * (amphipodType - 'A');
        for (var y = 2; y < 2 + _amphipods.Length / 4; y++)
            yield return (x, y);
    }

    public static AmphipodBurrow ReadFromInputFile(bool unfold = false)
    {
        var lines = InputFile.ReadAllLines();
        if (unfold)
        {
            lines = lines.Take(3)
                         .Append("  #D#C#B#A#")
                         .Append("  #D#B#A#C#")
                         .Concat(lines.Skip(3))
                         .ToArray();
        }

        return new AmphipodBurrow(lines.SelectMany((line, y) => line.Select((ch, x) => KeyValuePair.Create((x, y), ch)))
                                       .Where(x => x.Value != ' ')
                                       .ToImmutableDictionary(x => x.Key, x => x.Value));
    }
}
