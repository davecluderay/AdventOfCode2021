using System.Collections.Immutable;

namespace Aoc2021_Day23;

internal class Solution
{
    public string Title => "Day 23: Amphipod";

    public object? PartOne()
    {
        var burrow = AmphipodBurrow.ReadFromInputFile(unfold: false);
        var result = CalculateLowestEnergyCostToComplete(burrow);
        return result;
    }

    public object? PartTwo()
    {
        var burrow = AmphipodBurrow.ReadFromInputFile(unfold: true);
        var result = CalculateLowestEnergyCostToComplete(burrow);
        return result;
    }

    private long CalculateLowestEnergyCostToComplete(AmphipodBurrow burrow)
    {
        var cache = new Dictionary<int, long>();
        static int CalculateStateHash(AmphipodBurrow burrow) => string.Join(":", burrow.GetAmphipods().Select(a => a.ToString())).GetHashCode();
        long Calculate(AmphipodBurrow state, ImmutableHashSet<int> previousStateHashes)
        {
            var stateHash = CalculateStateHash(state);
            if (cache.ContainsKey(stateHash))
                return cache[stateHash];

            // Arrived at the current state twice, so it is not optimal.
            if (previousStateHashes.Contains(stateHash))
                return long.MaxValue;

            // If all amphipods are in their destinations, it's complete.
            var amphipods = state.GetAmphipods();
            if (amphipods.All(a => state.IsRoomFor(a.Position, a.Type)))
            {
                return 0L;
            }

            var lowestEnergyCostToComplete = long.MaxValue;
            var didRecurse = false;

            foreach (var amphipod in amphipods.Where(a => !state.IsRoomFor(a.Position, a.Type) || !state.IsRoomClearOfOtherAmphipodTypes(a.Type)))
            foreach (var nextPosition in state.FindReachableSpaces(amphipod.Position))
            {
                // Generally, the only valid destinations are the hallway or the correct destination room.
                if (!state.IsHallway(nextPosition.Position) &&
                    !state.IsRoomFor(nextPosition.Position, amphipod.Type))
                    continue;

                // Stopping at room entrances isn't allowed.
                if (state.IsRoomEntrance(nextPosition.Position))
                    continue;

                // If an amphipod is in the hallway, the only move it can make is into its destination room.
                if (state.IsHallway(amphipod.Position) && !state.IsRoomFor(nextPosition.Position, amphipod.Type))
                    continue;

                // If the destination room still has amphipods of the wrong type, the amphipod cannot enter yet.
                if (state.IsRoomFor(nextPosition.Position, amphipod.Type) &&
                    !state.IsRoomClearOfOtherAmphipodTypes(amphipod.Type))
                    continue;

                // Amphipods must go as far as possible into their destination room.
                if (state.IsRoomFor(nextPosition.Position, amphipod.Type) &&
                    !state.IsFurthestSpaceFromRoomEntrance(nextPosition.Position))
                    continue;

                // Recursive call.
                var subsequentCost = Calculate(state.MoveAmphipod(amphipod.Position, nextPosition.Position),
                                               previousStateHashes.Add(stateHash));
                var costToComplete = subsequentCost < long.MaxValue
                                         ? subsequentCost + 1L * nextPosition.NumberOfSteps * GetStepEnergyCost(amphipod.Type)
                                         : long.MaxValue;

                lowestEnergyCostToComplete = Math.Min(lowestEnergyCostToComplete, costToComplete);
            }

            cache[stateHash] = lowestEnergyCostToComplete;

            return lowestEnergyCostToComplete;
        }

        return Calculate(burrow, ImmutableHashSet.Create<int>());
    }

    private static int GetStepEnergyCost(char amphipodType)
        => (int)Math.Pow(10, amphipodType - 'A');
}
