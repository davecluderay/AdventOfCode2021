using static System.StringSplitOptions;

namespace Aoc2021_Day08;

internal class Solution
{
    public string Title => "Day 8: Seven Segment Search";

    public object? PartOne()
    {
        var outputs = from entry in ReadSampleData()
                      select DecodeOutput(entry.SignalPatterns, entry.OutputValues);
        return outputs.Sum(o => o.Count(c => c is '1' or '4' or '7' or '8'));
    }

    public object? PartTwo()
    {
        var outputs = from entry in ReadSampleData()
                      let output = DecodeOutput(entry.SignalPatterns, entry.OutputValues)
                      select Convert.ToInt32(output);
        return outputs.Sum();
    }

    private static string DecodeOutput(string[] signalPatterns, string[] outputValues)
    {
        var mappings = DeduceSignalToDigitMappings(signalPatterns);
        var output = new string(outputValues.Select(o => mappings[o]).ToArray());
        return output;
    }

    private static Dictionary<string, char> DeduceSignalToDigitMappings(string[] signalPatterns)
    {
        var signalPatternsByDigit = new Dictionary<char, string>
        {
            // 1, 4, 7 and 8 are the only digits with their respective number of segments.
            ['1'] = signalPatterns.Single(s => s.Length == 2),
            ['4'] = signalPatterns.Single(s => s.Length == 4),
            ['7'] = signalPatterns.Single(s => s.Length == 3),
            ['8'] = signalPatterns.Single(s => s.Length == 7)
        };

        // The central segment is identifiable by being in 4 and in all of the five segment digits.
        var centralSegment = signalPatternsByDigit['4'].Single(c => signalPatterns.Where(s => s.Length == 5)
                                                                                  .All(s => s.Contains(c)));
            
        // 0 doesn't have a central segment.
        signalPatternsByDigit['0'] = signalPatterns.Single(s => s.Length == 6 &&
                                                                !s.Contains(centralSegment));

        // 3 is the only five segment digit that is a superset of 1.
        signalPatternsByDigit['3'] = signalPatterns.Single(s => s.Length == 5 &&
                                                                signalPatternsByDigit['1'].All(s.Contains));
            
        // 9 is the only six segment digit (except for 0) that is a superset of 1.
        signalPatternsByDigit['9'] = signalPatterns.Single(s => s.Length == 6 &&
                                                                s != signalPatternsByDigit['0'] &&
                                                                signalPatternsByDigit['1'].All(s.Contains));
            
        // 6 is the only six segment digit (except for 0) that is not a superset of 1.
        signalPatternsByDigit['6'] = signalPatterns.Single(s => s.Length == 6 &&
                                                                s != signalPatternsByDigit['0'] &&
                                                                !signalPatternsByDigit['1'].All(s.Contains));
            
        // 5 is the only five segment digit (except for 3) that is a subset of 9.
        signalPatternsByDigit['5'] = signalPatterns.Single(s => s.Length == 5 &&
                                                                s != signalPatternsByDigit['3'] &&
                                                                s.All(signalPatternsByDigit['9'].Contains));
            
        // 2 is the only five segment digit (except for 3) that is not a subset of 9.
        signalPatternsByDigit['2'] = signalPatterns.Single(s => s.Length == 5 &&
                                                                s != signalPatternsByDigit['3'] &&
                                                                !s.All(signalPatternsByDigit['9'].Contains));
        
        return signalPatternsByDigit.ToDictionary(e => e.Value, e => e.Key);
    }

    private static IEnumerable<(string[] SignalPatterns, string[] OutputValues)> ReadSampleData()
    {
        string Alphabetise(string value)
            => new string(value.OrderBy(s => s).ToArray());

        IEnumerable<string> SplitOnWhitespace(string value)
            => value.Split(' ', TrimEntries | RemoveEmptyEntries);

        var results = from line in InputFile.ReadAllLines()
                      let delimiterAt = line.IndexOf('|')
                      let signalPatternsText = line.Substring(0, delimiterAt)
                      let outputValuesText = line.Substring(delimiterAt + 1)
                      select (SplitOnWhitespace(signalPatternsText).Select(Alphabetise).ToArray(),
                              SplitOnWhitespace(outputValuesText).Select(Alphabetise).ToArray());

        return results.ToArray();
    }
}
