using System.Text.RegularExpressions;

namespace Aoc2021_Day22;

internal record RebootStep(bool IsOn, Cuboid Cuboid)
{
    private static readonly Regex Pattern = new Regex(@"^(?<OnOff>on|off) x=(?<X1>[-]?\d+)[.][.](?<X2>[-]?\d+),y=(?<Y1>[-]?\d+)[.][.](?<Y2>[-]?\d+),z=(?<Z1>[-]?\d+)[.][.](?<Z2>[-]?\d+)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    public static RebootStep Parse(string input)
    {
        var match = Pattern.Match(input);
        if (!match.Success) throw new FormatException($"Could not parse cube state definition from: {input}");

        return new RebootStep(match.Groups["OnOff"].Value == "on",
            new Cuboid(Convert.ToInt32(match.Groups["X1"].Value),
                Convert.ToInt32(match.Groups["X2"].Value),
                Convert.ToInt32(match.Groups["Y1"].Value),
                Convert.ToInt32(match.Groups["Y2"].Value),
                Convert.ToInt32(match.Groups["Z1"].Value),
                Convert.ToInt32(match.Groups["Z2"].Value)));
    }
}