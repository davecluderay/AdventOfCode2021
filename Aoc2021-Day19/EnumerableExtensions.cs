namespace Aoc2021_Day19;

internal static class EnumerableExtensions
{
    public static IEnumerable<(T First, T Second)> InPairs<T>(this IReadOnlyList<T> list)
    {
        for (var i = 0; i < list.Count - 1; i++)
        for (var j = i + 1; j < list.Count; j++)
            yield return (list[i], list[j]);
    }
}
