namespace Aoc2021_Day10;

internal class Solution
{
    public string Title => "Day 10: Syntax Scoring";

    public object? PartOne()
    {
        var lines = InputFile.ReadAllLines();
        return lines.Sum(line => CheckLine(line).SyntaxErrorScore);
    }

    public object? PartTwo()
    {
        var lines = InputFile.ReadAllLines();
        var scores = lines.Select(line => CheckLine(line).AutocompletionScore)
                          .Where(s => s != 0)
                          .OrderBy(s => s)
                          .ToArray();
        return scores.Skip(scores.Length / 2).First();
    }

    private static (long SyntaxErrorScore, long AutocompletionScore) CheckLine(string line)
    {
        var autocompletion = new Stack<char>();
        foreach (var c in line)
        {
            switch (c)
            {
                case '(':
                case '[':
                case '{':
                case '<':
                    autocompletion.Push(LookupClosingCharacter(c));
                    break;
                case ')':
                case ']':
                case '}':
                case '>':
                    if (autocompletion.Count == 0 || autocompletion.Peek() != c)
                    {
                        return (LookupSyntaxErrorScore(c), 0);
                    }
                    autocompletion.Pop();
                    break;
            }
        }

        return (0, CalculateAutocompletionScore(autocompletion));
    }

    private static char LookupClosingCharacter(char openingCharacter)
        => openingCharacter switch
           {
               '(' => ')',
               '[' => ']',
               '{' => '}',
               '<' => '>',
               _   => throw new ArgumentException($"Unexpected: {openingCharacter}")
           };

    private static long LookupSyntaxErrorScore(char unexpectedCharacter)
        => unexpectedCharacter switch
           {
               ')' => 3,
               ']' => 57,
               '}' => 1197,
               '>' => 25137,
               _   => throw new ArgumentException($"Unexpected: {unexpectedCharacter}")
           };

    private static long CalculateAutocompletionScore(IEnumerable<char> autocompletion)
        => autocompletion.Aggregate(
            0L,
            (a, v) => a * 5 + v switch
                              {
                                  ')' => 1,
                                  ']' => 2,
                                  '}' => 3,
                                  '>' => 4,
                                  _   => throw new Exception("Unexpected")
                              });
}
