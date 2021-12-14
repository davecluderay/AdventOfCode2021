using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Aoc2021_Day14;

internal class Solution
{
    public string Title => "Day 14: Extended Polymerization";

    public object? PartOne()
    {
        var (template, rules) = ReadDataFromFile();
        var countsByElement = SynthesisePolymer(template, rules, 10);

        return countsByElement.Max(e => e.Count) - countsByElement.Min(e => e.Count);
    }

    public object? PartTwo()
    {
        var (template, rules) = ReadDataFromFile();
        var countsByElement = SynthesisePolymer(template, rules, 40);

        return countsByElement.Max(e => e.Count) - countsByElement.Min(e => e.Count);
    }

    private (string Template, (string Pair, char Insertion)[] Rules) ReadDataFromFile()
    {
        var matchPattern = new Regex("^[A-Z]+$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        var rulePattern = new Regex("^(?<Pair>[A-Z]{2}) -> (?<Insertion>[A-Z])+$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        var lines = InputFile.ReadAllLines();
        var template = lines.Single(l => matchPattern.IsMatch(l));
        var rules = lines.Select(l => rulePattern.Match(l))
                         .Where(m => m.Success)
                         .Select(m => (m.Groups["Pair"].Value, m.Groups["Insertion"].Value.Single()))
                         .ToArray();

        return (template, rules);
    }

    private static (char Element, long Count)[] SynthesisePolymer(string template, (string Pair, char Insertion)[] pairInsertionRules, int iterations)
    {
        // Not tracking the sequence, just the number of occurrences of each pair.
        var pairCounts = new ConcurrentDictionary<string, long>();
        for (var i = 1; i < template.Length; i++)
        {
            pairCounts.AddOrUpdate(template.Substring(i - 1, 2),
                                   _ => 1,
                                   (_, count) => count + 1);
        }

        // For fast lookup of insertion rules.
        var insertionsByPair = pairInsertionRules.ToDictionary(r => r.Pair, r => r.Insertion);

        // Perform the requested number of iterations.
        for (var step = 0; step < iterations; step++)
        {
            var removePairs = new List<(string pair, long count)>();
            var addPairs = new List<(string pair, long count)>();

            // Look for pairs that have an insertion rule and plan the actions to take.
            foreach (var (pair, count) in pairCounts)
            {
                if (!insertionsByPair.TryGetValue(pair, out var insertion)) continue;

                // Plan to replace each occurrence of this pair with two new pairs.
                removePairs.Add((pair, count));
                addPairs.Add(($"{pair[0]}{insertion}", count));
                addPairs.Add(($"{insertion}{pair[1]}", count));
            }

            // Apply the actions planned.
            foreach (var (pair, count) in removePairs)
                pairCounts[pair] -= count;

            foreach (var (pair, count) in addPairs)
                pairCounts.AddOrUpdate(pair, _ => count, (_, delta) => count + delta);
        }

        // Calculate and return the number of occurrences for each element.
        // Most occurrences participate in two pairs, but the first and last element only participate in one.
        var (first, last) = (template.First(), template.Last());
        var adjustments = new Dictionary<char, int>
                          {
                              [first] = 1,
                              [last] = 1
                          };

        var countsByElement = pairCounts.SelectMany(pc => pc.Key.Select(element => (element, count: pc.Value)))
                                        .GroupBy(x => x.element, x => x.count)
                                        .Select(g => (g.Key, (g.Sum() + adjustments.GetValueOrDefault(g.Key)) / 2))
                                        .ToArray();
        return countsByElement.ToArray();
    }
}
