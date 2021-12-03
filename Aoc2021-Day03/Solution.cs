using System.Text;

namespace Aoc2021_Day03;

internal class Solution
{
    public string Title => "Day 3: Binary Diagnostic";

    public object? PartOne()
    {
        var lines = InputFile.ReadAllLines();
        var digitCount = lines.First().Length;

        var gammaDigits = new StringBuilder(digitCount);
        var epsilonDigits = new StringBuilder(digitCount);

        for (var index = 0; index < digitCount; index++)
        {
            gammaDigits.Append(FindMostCommonDigit(lines, index).Digit);
            epsilonDigits.Append(FindLeastCommonDigit(lines, index).Digit);
        }

        var gamma = Convert.ToUInt64(gammaDigits.ToString(), 2);
        var epsilon = Convert.ToUInt64(epsilonDigits.ToString(), 2);

        return gamma * epsilon;
    }

    public object? PartTwo()
    {
        var lines = InputFile.ReadAllLines();
        var digitCount = lines.First().Length;

        var o2Candidates = lines.ToList();
        var co2Candidates = lines.ToList();

        for (var index = 0; index < digitCount; index++)
        {
            if (o2Candidates.Count > 1)
            {
                var mostCommonDigit = FindMostCommonDigit(o2Candidates, index);
                var leastCommonDigit = FindLeastCommonDigit(o2Candidates, index);
                var matchDigit = mostCommonDigit.Count == leastCommonDigit.Count ? '1' : mostCommonDigit.Digit;
                o2Candidates.RemoveAll(l => l[index] != matchDigit);
            }

            if (co2Candidates.Count > 1)
            {
                var mostCommonDigit = FindMostCommonDigit(co2Candidates, index);
                var leastCommonDigit = FindLeastCommonDigit(co2Candidates, index);
                var matchDigit = mostCommonDigit.Count == leastCommonDigit.Count ? '0' : leastCommonDigit.Digit;
                co2Candidates.RemoveAll(l => l[index] != matchDigit);
            }
        }

        var o2 = Convert.ToUInt64(o2Candidates.Single(), 2);
        var co2 = Convert.ToUInt64(co2Candidates.Single(), 2);

        return o2 * co2;
    }

    private static (char Digit, int Count) FindMostCommonDigit(IEnumerable<string> inputs, int index)
        => inputs.Select(s => s[index])
                 .GroupBy(c => c)
                 .OrderByDescending(g => g.Count())
                 .Select(g => (g.Key, g.Count()))
                 .First();

    private static (char Digit, int Count) FindLeastCommonDigit(IEnumerable<string> inputs, int index)
        => inputs.Select(s => s[index])
                 .GroupBy(c => c)
                 .OrderBy(g => g.Count())
                 .Select(g => (g.Key, g.Count()))
                 .First();
}
