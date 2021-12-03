using System.Text.RegularExpressions;

namespace Aoc2021_Day02;

internal record Instruction(string Action, int Value)
{
    private static readonly Regex Pattern = new("^(?<Action>(up|down|forward))\\s+(?<Value>\\d+)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture| RegexOptions.Singleline);

    public static Instruction Read(string input)
    {
        var match = Pattern.Match(input);
        if (!match.Success) throw new FormatException($"Unrecognised instruction format.");
        return new Instruction(match.Groups["Action"].Value, Convert.ToInt32(match.Groups["Value"].ToString()));
    }
}
